using NUnit.Framework;
using NetArchTest.Rules;
using System.Reflection;
using document_sharing_manager;

using document_sharing_manager.Core.Data;
using document_sharing_manager.Core;
namespace document_sharing_manager.Tests
{
    [TestFixture]
    public class ArchitectureTests
    {
        private static readonly Assembly TargetAssembly = typeof(DatabaseHelper).Assembly;

        [Test]
        public void UI_Should_Not_Have_Dependency_On_Infrastructure_Directly()
        {
            // Kiểm tra quy tắc: Các lớp trong UI không được phép phụ thuộc trực tiếp vào Infrastructure.
            // Điều này đảm bảo tính trừu tượng thông qua Interface (Core/Interfaces).
            var result = Types.InAssembly(TargetAssembly)
                .That()
                .ResideInNamespace("document_sharing_manager.UI")
                .ShouldNot()
                .HaveDependencyOn("document_sharing_manager.Infrastructure")
                .GetResult();

            Assert.That(result.IsSuccessful, Is.True, "Quy tắc kiến trúc bị vi phạm: Lớp UI đang phụ thuộc trực tiếp vào Infrastructure!");
        }

        [Test]
        public void Core_Should_Not_Have_Dependency_On_UI_Or_Infrastructure()
        {
            // Core là trung tâm, không được phụ thuộc vào bên ngoài.
            var result = Types.InAssembly(TargetAssembly)
                .That()
                .ResideInNamespace("document_sharing_manager.Core")
                .ShouldNot()
                .HaveDependencyOnAny("document_sharing_manager.UI", "document_sharing_manager.Infrastructure")
                .GetResult();

            Assert.That(result.IsSuccessful, Is.True, "Quy tắc kiến trúc bị vi phạm: Lớp Core không được phụ thuộc vào UI hoặc Infrastructure!");
        }
    }
}

