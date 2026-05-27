/**
 * DSM Server Health Monitor
 * Lightweight HTTP server that exposes real-time system metrics
 * for the CI/CD Observability Dashboard.
 */

const http = require("http");
const os = require("os");
const { execSync } = require("child_process");

const PORT = process.env.MONITOR_PORT || 5050;
const API_SECRET = process.env.API_SECRET_HEADER || "";

// ─── Helpers ──────────────────────────────────────────────

function getCpuUsage() {
  const cpus = os.cpus();
  let totalIdle = 0, totalTick = 0;
  cpus.forEach((cpu) => {
    for (const type in cpu.times) totalTick += cpu.times[type];
    totalIdle += cpu.times.idle;
  });
  const idle = totalIdle / cpus.length;
  const total = totalTick / cpus.length;
  const usage = ((1 - idle / total) * 100).toFixed(1);
  return {
    usagePercent: parseFloat(usage),
    cores: cpus.length,
    model: cpus[0] ? cpus[0].model.trim() : "Unknown",
  };
}

function getMemory() {
  const totalMB = Math.round(os.totalmem() / 1024 / 1024);
  const freeMB = Math.round(os.freemem() / 1024 / 1024);
  const usedMB = totalMB - freeMB;
  return {
    totalMB,
    usedMB,
    freeMB,
    usagePercent: parseFloat(((usedMB / totalMB) * 100).toFixed(1)),
  };
}

function getDisk() {
  try {
    // Windows: use wmic
    if (process.platform === "win32") {
      const raw = execSync(
        'wmic logicaldisk where "DeviceID=\'C:\'" get FreeSpace,Size /format:csv',
        { encoding: "utf8", timeout: 5000 }
      );
      const lines = raw.trim().split("\n").filter((l) => l.trim());
      const lastLine = lines[lines.length - 1];
      const parts = lastLine.split(",");
      if (parts.length >= 3) {
        const freeBytes = parseInt(parts[1]);
        const totalBytes = parseInt(parts[2]);
        const totalGB = parseFloat((totalBytes / 1073741824).toFixed(1));
        const freeGB = parseFloat((freeBytes / 1073741824).toFixed(1));
        const usedGB = parseFloat((totalGB - freeGB).toFixed(1));
        return {
          totalGB,
          usedGB,
          freeGB,
          usagePercent: parseFloat(((usedGB / totalGB) * 100).toFixed(1)),
        };
      }
    }
    // Linux/Mac: use df
    const raw = execSync("df -BG / | tail -1", {
      encoding: "utf8",
      timeout: 5000,
    });
    const parts = raw.trim().split(/\s+/);
    const totalGB = parseFloat(parts[1]);
    const usedGB = parseFloat(parts[2]);
    const freeGB = parseFloat(parts[3]);
    return {
      totalGB,
      usedGB,
      freeGB,
      usagePercent: parseFloat(((usedGB / totalGB) * 100).toFixed(1)),
    };
  } catch {
    return { totalGB: 0, usedGB: 0, freeGB: 0, usagePercent: 0, error: "Disk info unavailable" };
  }
}

function getDockerContainers() {
  try {
    const raw = execSync(
      'docker stats --no-stream --format "{{.Name}}|{{.CPUPerc}}|{{.MemUsage}}|{{.NetIO}}|{{.PIDs}}"',
      { encoding: "utf8", timeout: 15000 }
    );
    return raw
      .trim()
      .split("\n")
      .filter((l) => l.trim())
      .map((line) => {
        const [name, cpu, memUsage, netIO, pids] = line.split("|");
        return {
          name: name || "unknown",
          cpuPercent: cpu || "0%",
          memUsage: memUsage || "0B / 0B",
          netIO: netIO || "0B / 0B",
          pids: pids || "0",
        };
      });
  } catch {
    // Try docker ps as fallback (docker stats may not be available)
    try {
      const raw = execSync(
        'docker ps --format "{{.Names}}|{{.Status}}|{{.Image}}"',
        { encoding: "utf8", timeout: 10000 }
      );
      return raw
        .trim()
        .split("\n")
        .filter((l) => l.trim())
        .map((line) => {
          const [name, status, image] = line.split("|");
          return {
            name: name || "unknown",
            status: status || "unknown",
            image: image || "unknown",
            cpuPercent: "N/A",
            memUsage: "N/A",
            netIO: "N/A",
            pids: "N/A",
          };
        });
    } catch {
      return [];
    }
  }
}

function getNetwork() {
  const interfaces = os.networkInterfaces();
  const activeIfaces = [];
  for (const [name, addrs] of Object.entries(interfaces)) {
    for (const addr of addrs) {
      if (!addr.internal && addr.family === "IPv4") {
        activeIfaces.push({ name, address: addr.address, mac: addr.mac });
      }
    }
  }
  return {
    interfaces: activeIfaces,
    hostname: os.hostname(),
  };
}

function getUptime() {
  const uptimeSec = os.uptime();
  const days = Math.floor(uptimeSec / 86400);
  const hours = Math.floor((uptimeSec % 86400) / 3600);
  const minutes = Math.floor((uptimeSec % 3600) / 60);
  return {
    seconds: uptimeSec,
    formatted: `${days}d ${hours}h ${minutes}m`,
  };
}

// ─── Server ───────────────────────────────────────────────

const server = http.createServer((req, res) => {
  // CORS headers for dashboard
  res.setHeader("Access-Control-Allow-Origin", "*");
  res.setHeader("Access-Control-Allow-Methods", "GET, OPTIONS");
  res.setHeader("Access-Control-Allow-Headers", "Content-Type, X-Api-Secret");

  if (req.method === "OPTIONS") {
    res.writeHead(204);
    res.end();
    return;
  }

  if (req.url === "/api/server-stats" && req.method === "GET") {
    const stats = {
      cpu: getCpuUsage(),
      memory: getMemory(),
      disk: getDisk(),
      docker: getDockerContainers(),
      network: getNetwork(),
      uptime: getUptime(),
      platform: {
        os: os.type(),
        release: os.release(),
        arch: os.arch(),
      },
      timestamp: new Date().toISOString(),
    };

    res.writeHead(200, { "Content-Type": "application/json" });
    res.end(JSON.stringify(stats));
    return;
  }

  if (req.url === "/health" && req.method === "GET") {
    res.writeHead(200, { "Content-Type": "application/json" });
    res.end(JSON.stringify({ status: "ok", service: "dsm-server-monitor" }));
    return;
  }

  res.writeHead(404, { "Content-Type": "application/json" });
  res.end(JSON.stringify({ error: "Not found" }));
});

server.listen(PORT, "0.0.0.0", () => {
  console.log(`[DSM Monitor] Server health endpoint active on port ${PORT}`);
  console.log(`[DSM Monitor] GET /api/server-stats`);
  console.log(`[DSM Monitor] GET /health`);
});
