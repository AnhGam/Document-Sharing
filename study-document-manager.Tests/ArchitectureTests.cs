using NUnit.Framework;
using NetArchTest.Rules;
using System.Reflection;
using study_document_manager;

using study_document_manager.Core.Data;
using study_document_manager.Core;
namespace study_document_manager.Tests
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
                .ResideInNamespace("study_document_manager.UI")
                .ShouldNot()
                .HaveDependencyOn("study_document_manager.Infrastructure")
                .GetResult();

            Assert.That(result.IsSuccessful, Is.True, "Quy tắc kiến trúc bị vi phạm: Lớp UI đang phụ thuộc trực tiếp vào Infrastructure!");
        }

        [Test]
        public void Core_Should_Not_Have_Dependency_On_UI_Or_Infrastructure()
        {
            // Core là trung tâm, không được phụ thuộc vào bên ngoài.
            var result = Types.InAssembly(TargetAssembly)
                .That()
                .ResideInNamespace("study_document_manager.Core")
                .ShouldNot()
                .HaveDependencyOnAny("study_document_manager.UI", "study_document_manager.Infrastructure")
                .GetResult();

            Assert.That(result.IsSuccessful, Is.True, "Quy tắc kiến trúc bị vi phạm: Lớp Core không được phụ thuộc vào UI hoặc Infrastructure!");
        }
    }
}

