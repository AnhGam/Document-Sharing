FROM node:20-alpine
RUN apk add --no-cache docker-cli
WORKDIR /app
COPY package.json ./
RUN npm install --production
COPY index.js ./
EXPOSE 5050
CMD ["node", "index.js"]
