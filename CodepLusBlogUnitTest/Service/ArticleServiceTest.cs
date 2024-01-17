using CodePlusBlog.Context;
using CodePlusBlog.IService;
using CodePlusBlog.Model;
using CodePlusBlog.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ModalLayer.Model;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CodepLusBlogUnitTest.Service
{
    public class ArticleServiceTest
    {
        private Mock<IFileService> _mockFileService;
        private Mock<RepositoryContext> _mockContxet;
        private Mock<IWebHostEnvironment> _mockEnvironment;
        private Mock<IFormFileCollection> _mockFormFiles;
        private Mock<IDbContextTransactionWrapper> _mockContextTransactionWrapper;
        public ArticleServiceTest()
        {
            _mockContxet = new Mock<RepositoryContext>();
            _mockFileService = new Mock<IFileService>();
            _mockEnvironment = new Mock<IWebHostEnvironment>();
            _mockFormFiles = new Mock<IFormFileCollection>();
            _mockContextTransactionWrapper = new Mock<IDbContextTransactionWrapper>();
        }

        [Fact]
        public async Task SaveArticleService_SavesNewArticleWithFileAndTextFile_Success()
        {
            var data = new List<ContentList>().AsQueryable();
            List<ContentList> contents = new List<ContentList>();
            var contentlist = new ContentList
            {
                ContentId = 0
            };
            var mockset = new Mock<DbSet<ContentList>>();
            Mock<IFormFile> mockFile1 = new Mock<IFormFile>();
            Mock<IFormFile> mockFile2 = new Mock<IFormFile>();
            mockset.As<IQueryable<ContentList>>().Setup(x => x.Provider).Returns(data.Provider);
            mockset.As<IQueryable<ContentList>>().Setup(x => x.ElementType).Returns(data.ElementType);
            mockset.As<IQueryable<ContentList>>().Setup(x => x.Expression).Returns(data.Expression);
            mockset.As<IQueryable<ContentList>>().Setup(x => x.GetEnumerator()).Returns(() => data.GetEnumerator());
            
            _mockFileService.Setup(x => x.SaveFile(It.IsAny<string>(), It.IsAny<List<Files>>(), It.IsAny<IFormFileCollection>(), It.IsAny<string>())).Returns(string.Empty);
            _mockFileService.Setup(x => x.SaveTextFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(string.Empty);

            mockFile1.Setup(x => x.Name).Returns("file1.txt");
            mockFile2.Setup(x => x.ContentType).Returns("image/jpeg");
            _mockFormFiles.Setup(x => x.Count).Returns(2);
            _mockFormFiles.Setup(x => x.GetEnumerator()).Returns(new List<IFormFile> { mockFile1.Object, mockFile2.Object }.GetEnumerator());
            _mockContxet.Setup(x => x.Set<ContentList>()).Returns(mockset.Object);
           //var articleservice = new ArticleService(_mockContxet.Object, _mockFileService.Object, _mockEnvironment.Object, _mockContextTransactionWrapper.Object);
           // var result = await articleservice.SaveArticleService(contentlist, _mockFormFiles.Object);

            _mockContextTransactionWrapper.Verify(t => t.BeginTransaction(), Times.Once);
            _mockContextTransactionWrapper.Verify(t => t.Commit(), Times.Once);
            _mockContextTransactionWrapper.Verify(t => t.Rollback(), Times.Never);
            _mockContextTransactionWrapper.Verify(t => t.Dispose(), Times.Once);
        }
    }
}

