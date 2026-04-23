using NUnit.Framework;
using JetBrains.dotMemoryUnit;
using System.Windows.Forms;
using study_document_manager;

namespace study_document_manager.Tests
{
    [TestFixture]
    public class PerformanceTests
    {
        [Test]
        [DotMemoryUnit]
        public void Dashboard_Should_Not_Leak_Memory_After_Closing()
        {
            // Bài test này dùng dotMemory Unit để kiểm tra rò rỉ bộ nhớ.
            // Quy trình: Khởi tạo Dashboard -> Hiển thị -> Đóng -> Kiểm tra xem object có bị kẹt lại không.
            
            RunMemoryCheck();
        }

        private void RunMemoryCheck()
        {
            // Chụp snapshot trước
            var memoryBefore = dotMemory.Check();

            // Thực hiện hành động (Mở và đóng Form)
            Dashboard dashboard = new Dashboard();
            dashboard.Show();
            dashboard.Close();
            dashboard.Dispose();
            dashboard = null;

            // Chụp snapshot sau và so sánh
            dotMemory.Check(memory =>
            {
                // Kiểm tra xem lớp Dashboard có còn tồn tại trong bộ nhớ không
                var instanceCount = memory.GetObjects(where => where.Type.Is<Dashboard>()).ObjectsCount;
                Assert.That(instanceCount, Is.EqualTo(0), "Phát hiện rò rỉ bộ nhớ: Lớp Dashboard vẫn tồn tại sau khi đã đóng!");
            });
        }
    }
}
