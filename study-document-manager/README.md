# study-document-manager
### 1. Thống nhất các yếu tố cốt lõi

#### Mục tiêu và phạm vi
- **Mục tiêu**: Ứng dụng giúp sinh viên, giáo viên, và nhân viên tổ chức tài liệu học tập (PDF, Word, slide, v.v.), tìm kiếm nhanh, mở file trực tiếp, thêm ghi chú, và hiển thị thống kê.
- **Phạm vi**:
  - **Đối tượng người dùng**: Sinh viên, giáo viên, nhân viên.
  - **Tính năng cốt lõi** (bắt buộc):
    1. Thêm/sửa/xóa tài liệu.
    2. Tìm kiếm theo tên, môn học, ghi chú.
    3. Mở file trực tiếp (bắt buộc).
    4. Ghi chú cho tài liệu.
    5. Xuất danh sách tài liệu ra file `.txt` hoặc `.csv`.
    6. Thống kê số lượng tài liệu (biểu đồ).
  - **Tính năng tùy chọn** (nâng cao, làm nếu có thời gian):
    1. Kéo-thả file để thêm tài liệu.
    2. Đánh dấu tài liệu quan trọng (sao vàng).
  - **Không có yêu cầu cụ thể từ giảng viên** ngoài việc làm ứng dụng C# Windows Forms, nộp báo cáo, và demo.

#### Yêu cầu kỹ thuật
- **Công nghệ**:
  - **C# Windows Forms**: .NET Framework.
  - **Cơ sở dữ liệu**: SQL Server (bạn đang sử dụng).
  - **Thư viện bên ngoài**: LiveCharts.WinForms (dùng cho biểu đồ, nếu có thời gian).
- **Không giới hạn kích thước ứng dụng** (dung lượng database, số lượng tài liệu).

#### Cấu trúc dữ liệu
- **Bảng SQL Server**: Bảng `tai-lieu` với các trường:
  - `id` (int, khóa chính, tự động tăng).
  - `ten` (nvarchar, tên tài liệu).
  - `mon-hoc` (nvarchar, môn học: toán, lập trình, anh văn, v.v.).
  - `loai` (nvarchar, loại tài liệu: slide, bài tập, đề thi, v.v.).
  - `duong-dan` (nvarchar, đường dẫn file).
  - `ghi-chu` (nvarchar, ghi chú).
  - `ngay-them` (datetime, ngày thêm).
  - `kich-thuoc` (float, kích thước file, tính bằng MB).
  - `tac-gia` (nvarchar, tác giả tài liệu).
  - `quan-trong` (bit, đánh dấu tài liệu quan trọng: 1 = quan trọng, 0 = không).
- **Lưu ý**: Bạn chưa quen với SQL Server, tôi sẽ hướng dẫn cách kết nối và sử dụng chi tiết.

#### Giao diện
- **Phong cách**: Hiện đại, tối giản, màu sắc hài hòa.
- **Bố cục**:
  - **MainForm**:
    - **DataGridView**: Hiển thị danh sách tài liệu (cột: `ten`, `mon-hoc`, `loai`, `ngay-them`, `ghi-chu`, `kich-thuoc`, `tac-gia`, `quan-trong`).
    - **TextBox tìm kiếm** + Button tìm kiếm.
    - **ComboBox**: Lọc theo `mon-hoc` và `loai`.
    - **Button**: Thêm, Sửa, Xóa, Mở file, Xuất dữ liệu, Thống kê.
    - **Panel**: Hiển thị biểu đồ (LiveCharts, nếu có thời gian).
  - **AddEditForm**:
    - **TextBox**: `ten`, `ghi-chu`, `tac-gia`.
    - **ComboBox**: `mon-hoc`, `loai`.
    - **Button**: Chọn file (OpenFileDialog), Lưu, Hủy.
    - **CheckBox**: `quan-trong`.
- **Phối màu**:
  - **Nền Form**: Xanh dương nhạt (#E3F2FD).
  - **DataGridView**: Xen kẽ trắng (#FFFFFF) và xám nhạt (#F5F5F5).
  - **Button**:
    - Lưu/Thêm: Xanh lá (#4CAF50).
    - Xóa: Đỏ (#F44336).
    - Mở file/Xuất dữ liệu: Xanh dương (#2196F3).
  - **Icon/Ảnh**: Sử dụng biểu tượng cho loại tài liệu (PDF, Word, PowerPoint) trong DataGridView, lấy từ FontAwesome hoặc ảnh tùy chỉnh.
- **Yêu cầu**: Giao diện đẹp, hiện đại, nhưng ưu tiên code chạy ổn định trước.

#### Thời gian
- **Tổng thời gian**: 90 tiết (~67.5 giờ), nhưng mục tiêu hoàn thành trong ~30 tiết (~22-25 giờ).
- **Phân bổ**:
  - Thiết kế giao diện: 3-4 giờ.
  - Kết nối SQL Server và cấu trúc bảng: 2-3 giờ.
  - Tính năng cốt lõi (thêm/sửa/xóa, tìm kiếm, mở file, ghi chú): 10-12 giờ.
  - Thống kê (biểu đồ) và xuất dữ liệu: 3-4 giờ.
  - Kiểm thử và hoàn thiện: 3-4 giờ.
  - Tổng: ~21-27 giờ.

#### Kỹ năng của bạn
- **Đã quen**: Windows Forms, DataGridView, OpenFileDialog (nhưng chưa dùng 4 tháng).
- **Chưa quen**: SQL Server, cần hướng dẫn kết nối và sử dụng.
- **Thoải mái**: Với thư viện bên ngoài (LiveCharts).
- **Ưu tiên**: Code chạy được trước, sau đó tối ưu giao diện.
- **Hướng dẫn**: Cần hướng dẫn chi tiết từng tính năng, đặc biệt kết nối SQL Server.

#### Trình bày
- **Yêu cầu**: Nộp báo cáo (Word/PDF) và demo ứng dụng.
- **Không giới hạn**: Báo cáo nêu mục tiêu, tính năng, cách sử dụng, và kết quả.

---

### 2. Thiết kế giao diện chi tiết
#### MainForm
- **Kích thước**: 1000x600 pixel.
- **Bố cục**:
  - **Phần trên** (10% chiều cao):
    - **TextBox tìm kiếm** (rộng 70%, phông Segoe UI, kích thước 12).
    - **Button "tìm-kiếm"** (xanh dương #2196F3, 100x30).
    - **ComboBox mon-hoc** (rộng 15%, danh sách: toán, lập trình, anh văn, vật lý, v.v.).
    - **ComboBox loai** (rộng 15%, danh sách: slide, bài tập, đề thi, tài liệu khác).
  - **Phần giữa** (60% chiều cao):
    - **DataGridView**:
      - Cột: `ten` (20%), `mon-hoc` (15%), `loai` (15%), `ngay-them` (15%), `ghi-chu` (20%), `kich-thuoc` (10%), `tac-gia` (15%), `quan-trong` (5%, hiển thị sao vàng).
      - Xen kẽ trắng (#FFFFFF) và xám nhạt (#F5F5F5).
      - Hỗ trợ double-click để mở file.
      - Cột biểu tượng: PDF (đỏ #F44336), Word (xanh dương #2196F3), PowerPoint (cam #FF9800).
  - **Phần dưới** (20% chiều cao):
    - **Panel biểu đồ**: Hiển thị biểu đồ cột (số lượng tài liệu theo `mon-hoc`/`loai`).
    - **Button** (kích thước 100x30):
      - Thêm (xanh lá #4CAF50).
      - Sửa (xanh dương #2196F3).
      - Xóa (đỏ #F44336).
      - Mở file (xanh dương #2196F3).
      - Xuất dữ liệu (xanh dương #2196F3).
      - Thống kê (xanh lá #4CAF50).
  - **Nền**: Xanh dương nhạt (#E3F2FD), viền mỏng (#B0BEC5).
- **Phong cách**:
  - Phông chữ: Segoe UI.
  - Góc bo tròn cho Button (BorderRadius = 5).
  - Biểu tượng từ FontAwesome (PDF, Word, PowerPoint, sao vàng).

#### AddEditForm
- **Kích thước**: 400x500 pixel.
- **Bố cục**:
  - **TextBox**:
    - `ten` (rộng 90%, cao 30).
    - `ghi-chu` (rộng 90%, cao 100, RichTextBox).
    - `tac-gia` (rộng 90%, cao 30).
  - **ComboBox**:
    - `mon-hoc` (rộng 90%).
    - `loai` (rộng 90%).
  - **Button**:
    - Chọn file (xanh dương #2196F3, 100x30).
    - Lưu (xanh lá #4CAF50, 100x30).
    - Hủy (đỏ #F44336, 100x30).
  - **CheckBox**: `quan-trong` (hiển thị sao vàng).
  - **Nền**: Trắng (#FFFFFF), viền xanh nhạt (#BBDEFB).
- **Phong cách**: Phông Segoe UI, góc bo tròn.

#### Biểu tượng
- **Nguồn**: FontAwesome (NuGet: `FontAwesome.Sharp`).
- **Loại**:
  - PDF: Icon file-pdf (đỏ #F44336).
  - Word: Icon file-word (xanh dương #2196F3).
  - PowerPoint: Icon file-powerpoint (cam #FF9800).
  - Quan trọng: Icon star (vàng #FFCA28).
- **Cách dùng**: Thêm cột Image trong DataGridView, gán icon dựa trên `loai` và `quan-trong`.

---

### 3. Cấu trúc SQL Server
- **Tên bảng**: `tai-lieu`.
- **Cấu trúc**:
```sql
CREATE TABLE tai_lieu (
    id INT PRIMARY KEY IDENTITY(1,1),
    ten NVARCHAR(200) NOT NULL,
    mon_hoc NVARCHAR(100),
    loai NVARCHAR(100),
    duong_dan NVARCHAR(500) NOT NULL,
    ghi_chu NVARCHAR(1000),
    ngay_them DATETIME DEFAULT GETDATE(),
    kich_thuoc FLOAT,
    tac_gia NVARCHAR(100),
    quan_trong BIT DEFAULT 0
);
```
- **Giải thích**:
  - `id`: Khóa chính, tự động tăng.
  - `ten`: Tên tài liệu (bắt buộc).
  - `mon_hoc`, `loai`: Có thể để trống (NULL).
  - `duong_dan`: Đường dẫn file (bắt buộc).
  - `ghi_chu`: Ghi chú dài (tối đa 1000 ký tự).
  - `ngay_them`: Mặc định thời gian hiện tại.
  - `kich_thuoc`: Kích thước file (MB, có thể để trống).
  - `tac_gia`: Tên tác giả (có thể để trống).
  - `quan_trong`: 1 = quan trọng, 0 = không (mặc định 0).

---

### 4. Hướng dẫn triển khai từng tính năng
Dưới đây là hướng dẫn chi tiết cho từng tính năng, ưu tiên code chạy được, kèm hướng dẫn kết nối SQL Server. Tất cả tên biến và cột đều viết thường, dùng dấu gạch ngang.

#### Tính năng 1: Thêm/Sửa/Xóa tài liệu
- **Mô tả**: Thêm, sửa, xóa tài liệu qua AddEditForm.
- **Hướng dẫn**:
  - **Kết nối SQL Server**:
    - Chuỗi kết nối (thay đổi tùy máy bạn):
      ```csharp
      string connection_string = "Server=localhost;Database=quan_ly_tai_lieu;Trusted_Connection=True;";
      ```
    - Cài thư viện: NuGet package `System.Data.SqlClient`.
    - Kết nối cơ bản:
      ```csharp
      using System.Data.SqlClient;

      SqlConnection conn = new SqlConnection(connection_string);
      try
      {
          conn.Open();
          // Thực hiện truy vấn
          conn.Close();
      }
      catch (Exception ex)
      {
          MessageBox.Show("lỗi kết nối: " + ex.Message);
      }
      ```
  - **Thêm tài liệu** (AddEditForm):
    - Chọn file bằng `OpenFileDialog`:
      ```csharp
      OpenFileDialog open_file_dialog = new OpenFileDialog();
      open_file_dialog.Filter = "Files|*.pdf;*.doc;*.docx;*.ppt;*.pptx";
      if (open_file_dialog.ShowDialog() == DialogResult.OK)
      {
          string duong_dan = open_file_dialog.FileName;
          double kich_thuoc = new System.IO.FileInfo(duong_dan).Length / (1024.0 * 1024.0); // MB
          txt_duong_dan.Text = duong_dan;
          txt_kich_thuoc.Text = kich_thuoc.ToString("F2");
      }
      ```
    - Lưu vào SQL Server:
      ```csharp
      string query = "INSERT INTO tai_lieu (ten, mon_hoc, loai, duong_dan, ghi_chu, kich_thuoc, tac_gia, quan_trong) VALUES (@ten, @mon_hoc, @loai, @duong_dan, @ghi_chu, @kich_thuoc, @tac_gia, @quan_trong)";
      using (SqlConnection conn = new SqlConnection(connection_string))
      {
          conn.Open();
          SqlCommand cmd = new SqlCommand(query, conn);
          cmd.Parameters.AddWithValue("@ten", txt_ten.Text);
          cmd.Parameters.AddWithValue("@mon_hoc", cbo_mon_hoc.SelectedItem?.ToString() ?? "");
          cmd.Parameters.AddWithValue("@loai", cbo_loai.SelectedItem?.ToString() ?? "");
          cmd.Parameters.AddWithValue("@duong_dan", txt_duong_dan.Text);
          cmd.Parameters.AddWithValue("@ghi_chu", txt_ghi_chu.Text);
          cmd.Parameters.AddWithValue("@kich_thuoc", txt_kich_thuoc.Text == "" ? DBNull.Value : Convert.ToDouble(txt_kich_thuoc.Text));
          cmd.Parameters.AddWithValue("@tac_gia", txt_tac_gia.Text);
          cmd.Parameters.AddWithValue("@quan_trong", chk_quan_trong.Checked ? 1 : 0);
          cmd.ExecuteNonQuery();
          conn.Close();
      }
      ```
  - **Sửa tài liệu**:
    - Load dữ liệu vào AddEditForm:
      ```csharp
      string query = "SELECT * FROM tai_lieu WHERE id = @id";
      using (SqlConnection conn = new SqlConnection(connection_string))
      {
          conn.Open();
          SqlCommand cmd = new SqlCommand(query, conn);
          cmd.Parameters.AddWithValue("@id", selected_id);
          SqlDataReader reader = cmd.ExecuteReader();
          if (reader.Read())
          {
              txt_ten.Text = reader["ten"].ToString();
              cbo_mon_hoc.SelectedItem = reader["mon_hoc"].ToString();
              cbo_loai.SelectedItem = reader["loai"].ToString();
              txt_duong_dan.Text = reader["duong_dan"].ToString();
              txt_ghi_chu.Text = reader["ghi_chu"].ToString();
              txt_kich_thuoc.Text = reader["kich_thuoc"].ToString();
              txt_tac_gia.Text = reader["tac_gia"].ToString();
              chk_quan_trong.Checked = Convert.ToBoolean(reader["quan_trong"]);
          }
          conn.Close();
      }
      ```
    - Cập nhật:
      ```csharp
      string query = "UPDATE tai_lieu SET ten = @ten, mon_hoc = @mon_hoc, loai = @loai, duong_dan = @duong_dan, ghi_chu = @ghi_chu, kich_thuoc = @kich_thuoc, tac_gia = @tac_gia, quan_trong = @quan_trong WHERE id = @id";
      using (SqlConnection conn = new SqlConnection(connection_string))
      {
          conn.Open();
          SqlCommand cmd = new SqlCommand(query, conn);
          cmd.Parameters.AddWithValue("@ten", txt_ten.Text);
          cmd.Parameters.AddWithValue("@mon_hoc", cbo_mon_hoc.SelectedItem?.ToString() ?? "");
          cmd.Parameters.AddWithValue("@loai", cbo_loai.SelectedItem?.ToString() ?? "");
          cmd.Parameters.AddWithValue("@duong_dan", txt_duong_dan.Text);
          cmd.Parameters.AddWithValue("@ghi_chu", txt_ghi_chu.Text);
          cmd.Parameters.AddWithValue("@kich_thuoc", txt_kich_thuoc.Text == "" ? DBNull.Value : Convert.ToDouble(txt_kich_thuoc.Text));
          cmd.Parameters.AddWithValue("@tac_gia", txt_tac_gia.Text);
          cmd.Parameters.AddWithValue("@quan_trong", chk_quan_trong.Checked ? 1 : 0);
          cmd.Parameters.AddWithValue("@id", selected_id);
          cmd.ExecuteNonQuery();
          conn.Close();
      }
      ```
  - **Xóa tài liệu**:
    - Xác nhận trước khi xóa:
      ```csharp
      if (MessageBox.Show("xóa tài liệu này?", "xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
      {
          string query = "DELETE FROM tai_lieu WHERE id = @id";
          using (SqlConnection conn = new SqlConnection(connection_string))
          {
              conn.Open();
              SqlCommand cmd = new SqlCommand(query, conn);
              cmd.Parameters.AddWithValue("@id", selected_id);
              cmd.ExecuteNonQuery();
              conn.Close();
          }
      }
      ```
  - **Hiển thị danh sách** (MainForm):
    ```csharp
    private void load_data()
    {
        using (SqlConnection conn = new SqlConnection(connection_string))
        {
            conn.Open();
            string query = "SELECT * FROM tai_lieu";
            SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            data_grid_view_tai_lieu.DataSource = dt;
            conn.Close();
        }
    }
    ```

#### Tính năng 2: Tìm kiếm và lọc
- **Tìm kiếm**:
  - Tìm theo `ten`, `mon_hoc`, `ghi_chu`:
    ```csharp
    private void btn_tim_kiem_Click(object sender, EventArgs e)
    {
        using (SqlConnection conn = new SqlConnection(connection_string))
        {
            conn.Open();
            string query = "SELECT * FROM tai_lieu WHERE ten LIKE @keyword OR mon_hoc LIKE @keyword OR ghi_chu LIKE @keyword";
            SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
            adapter.SelectCommand.Parameters.AddWithValue("@keyword", "%" + txt_tim_kiem.Text + "%");
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            data_grid_view_tai_lieu.DataSource = dt;
            conn.Close();
        }
    }
    ```
- **Lọc**:
  - Lọc theo `mon_hoc` hoặc `loai`:
    ```csharp
    private void cbo_mon_hoc_SelectedIndexChanged(object sender, EventArgs e)
    {
        using (SqlConnection conn = new SqlConnection(connection_string))
        {
            conn.Open();
            string query = "SELECT * FROM tai_lieu WHERE mon_hoc = @mon_hoc OR @mon_hoc = ''";
            SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
            adapter.SelectCommand.Parameters.AddWithValue("@mon_hoc", cbo_mon_hoc.SelectedItem?.ToString() ?? "");
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            data_grid_view_tai_lieu.DataSource = dt;
            conn.Close();
        }
    }
    ```
  - Tương tự cho `loai`.

#### Tính năng 3: Mở file trực tiếp
- **Mô tả**: Double-click hàng trong DataGridView để mở file.
- **Code**:
  ```csharp
  private void data_grid_view_tai_lieu_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
  {
      if (e.RowIndex >= 0)
      {
          string duong_dan = data_grid_view_tai_lieu.Rows[e.RowIndex].Cells["duong_dan"].Value.ToString();
          try
          {
              System.Diagnostics.Process.Start(duong_dan);
          }
          catch (Exception ex)
          {
              MessageBox.Show("không thể mở file: " + ex.Message);
          }
      }
  }
  ```

#### Tính năng 4: Ghi chú
- **Mô tả**: Lưu ghi chú trong `ghi_chu` (RichTextBox trong AddEditForm).
- **Code**: Đã tích hợp trong Thêm/Sửa.

#### Tính năng 5: Xuất dữ liệu
- **Mô tả**: Xuất danh sách tài liệu ra file `.csv`.
- **Code**:
  ```csharp
  private void btn_xuat_Click(object sender, EventArgs e)
  {
      SaveFileDialog save_file_dialog = new SaveFileDialog();
      save_file_dialog.Filter = "CSV files (*.csv)|*.csv";
      if (save_file_dialog.ShowDialog() == DialogResult.OK)
      {
          using (SqlConnection conn = new SqlConnection(connection_string))
          {
              conn.Open();
              string query = "SELECT * FROM tai_lieu";
              SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
              DataTable dt = new DataTable();
              adapter.Fill(dt);

              using (System.IO.StreamWriter writer = new System.IO.StreamWriter(save_file_dialog.FileName))
              {
                  writer.WriteLine("id,ten,mon_hoc,loai,duong_dan,ghi_chu,ngay_them,kich_thuoc,tac_gia,quan_trong");
                  foreach (DataRow row in dt.Rows)
                  {
                      writer.WriteLine($"{row["id"]},\"{row["ten"]}\",\"{row["mon_hoc"]}\",\"{row["loai"]}\",\"{row["duong_dan"]}\",\"{row["ghi_chu"]}\",\"{row["ngay_them"]}\",{row["kich_thuoc"]},\"{row["tac_gia"]}\",{row["quan_trong"]}");
                  }
              }
              conn.Close();
              MessageBox.Show("xuất file thành công!");
          }
      }
  }
  ```

#### Tính năng 6: Thống kê
- **Mô tả**: Hiển thị biểu đồ số lượng tài liệu theo `mon_hoc` hoặc `loai`.
- **Cài đặt LiveCharts**:
  - NuGet: `Install-Package LiveCharts.WinForms`.
- **Code**:
  ```csharp
  private void btn_thong_ke_Click(object sender, EventArgs e)
  {
      using (SqlConnection conn = new SqlConnection(connection_string))
      {
          conn.Open();
          string query = "SELECT mon_hoc, COUNT(*) as so_luong FROM tai_lieu GROUP BY mon_hoc";
          SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
          DataTable dt = new DataTable();
          adapter.Fill(dt);

          var chart = new LiveCharts.WinForms.CartesianChart
          {
              Dock = DockStyle.Fill
          };
          panel_chart.Controls.Clear();
          panel_chart.Controls.Add(chart);

          chart.Series = new LiveCharts.SeriesCollection
          {
              new LiveCharts.Wpf.ColumnSeries
              {
                  Title = "số lượng tài liệu",
                  Values = new LiveCharts.ChartValues<double>()
              }
          };

          chart.AxisX.Labels = new List<string>();
          foreach (DataRow row in dt.Rows)
          {
              chart.Series[0].Values.Add(Convert.ToDouble(row["so_luong"]));
              chart.AxisX.Labels.Add(row["mon_hoc"].ToString());
          }
          conn.Close();
      }
  }
  ```
- **Lưu ý**: Thêm Panel `panel_chart` vào MainForm.

#### Tính năng tùy chọn: Kéo-thả file
- **Mô tả**: Kéo file vào DataGridView để thêm tài liệu.
- **Code**:
  ```csharp
  public MainForm()
  {
      InitializeComponent();
      data_grid_view_tai_lieu.AllowDrop = true;
      data_grid_view_tai_lieu.DragEnter += (s, e) =>
      {
          if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
      };
      data_grid_view_tai_lieu.DragDrop += (s, e) =>
      {
          string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
          foreach (string file in files)
          {
              AddEditForm form = new AddEditForm();
              form.txt_duong_dan.Text = file;
              form.txt_kich_thuoc.Text = (new System.IO.FileInfo(file).Length / (1024.0 * 1024.0)).ToString("F2");
              if (form.ShowDialog() == DialogResult.OK)
              {
                  load_data();
              }
          }
      };
  }
  ```

#### Tính năng tùy chọn: Đánh dấu tài liệu quan trọng
- **Mô tả**: CheckBox trong AddEditForm để đánh dấu `quan_trong`, hiển thị sao vàng.
- **Code**:
  - Đã tích hợp trong Thêm/Sửa.
  - Thêm cột Image trong DataGridView:
    ```csharp
    private void data_grid_view_tai_lieu_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
    {
        if (data_grid_view_tai_lieu.Columns[e.ColumnIndex].Name == "quan_trong")
        {
            if (e.Value != null && Convert.ToBoolean(e.Value))
            {
                e.Value = Properties.Resources.star_icon; // Icon sao vàng từ Resources
                e.FormattingApplied = true;
            }
        }
        else if (data_grid_view_tai_lieu.Columns[e.ColumnIndex].Name == "icon")
        {
            string loai = data_grid_view_tai_lieu.Rows[e.RowIndex].Cells["loai"].Value?.ToString();
            if (loai == "slide") e.Value = Properties.Resources.powerpoint_icon;
            else if (loai == "bài tập") e.Value = Properties.Resources.word_icon;
            else if (loai == "đề thi") e.Value = Properties.Resources.pdf_icon;
            e.FormattingApplied = true;
        }
    }
    ```

---

### 5. Quy trình phát triển (tối ưu cho 30 tiết)
- **Ngày 1-2 (~3-4 giờ)**:
  - Thiết kế giao diện MainForm và AddEditForm (Visual Studio Designer).
  - Cài SQL Server, tạo bảng `tai_lieu`.
- **Ngày 3-5 (~6-8 giờ)**:
  - Kết nối SQL Server, triển khai Thêm/Sửa/Xóa.
  - Kiểm tra lỗi (file không tồn tại, nhập liệu không hợp lệ).
- **Ngày 6-8 (~6-8 giờ)**:
  - Triển khai Tìm kiếm, Lọc, Mở file.
  - Thêm biểu tượng cho DataGridView (PDF, Word, PowerPoint, sao vàng).
- **Ngày 9-10 (~3-4 giờ)**:
  - Triển khai Thống kê (biểu đồ với LiveCharts).
  - Triển khai Xuất dữ liệu (.csv).
- **Ngày 11-12 (~3-4 giờ)**:
  - Kiểm thử toàn bộ tính năng.
  - Tối ưu giao diện (màu sắc, bố cục).
  - Viết báo cáo và chuẩn bị demo.

---

### 6. Lời khuyên
1. **Ưu tiên logic**: Hoàn thiện Thêm/Sửa/Xóa và Mở file trước.
2. **Giao diện**: Dùng phông Segoe UI, màu sắc hiện đại, icon FontAwesome.
3. **Kiểm thử**: Test với 10 tài liệu (PDF, Word, PowerPoint).
4. **Báo cáo**: Ngắn gọn, nêu mục tiêu, tính năng, cách dùng, kèm ảnh chụp màn hình.
5. **Tính năng nâng cao**: Thêm kéo-thả file nếu có thời gian.

---

### 7. Hỗ trợ tiếp theo
- **Code mẫu chi tiết**: Yêu cầu tính năng cụ thể (ví dụ: AddEditForm).
- **Hướng dẫn cài đặt**:
  - **SQL Server**: Cài SQL Server Express và Management Studio.
  - **LiveCharts**: Cấu hình NuGet và vẽ biểu đồ.
  - **FontAwesome**: Thêm icon vào DataGridView.
- **Tài liệu**:
  - File SQL tạo bảng.
  - Báo cáo mẫu.

Hãy cho tôi biết bạn muốn bắt đầu từ đâu (thiết kế giao diện, kết nối SQL Server) hoặc cần giải thích thêm (FontAwesome, xử lý lỗi SQL). Bạn có muốn tôi cung cấp file SQL hoặc báo cáo mẫu không?
