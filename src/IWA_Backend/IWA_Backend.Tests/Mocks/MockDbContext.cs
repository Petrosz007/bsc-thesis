using IWA_Backend.API.BusinessLogic.Entities;
using IWA_Backend.API.Contexts;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IWA_Backend.Tests.Mocks
{
    internal class MockDbContextBuilder
    {
        public List<User> Users { get; init; } = new();
        public List<Appointment> Appointments { get; init; } = new();
        public List<Category> Categories { get; init; } = new();
        public List<ContractorPage> ContractorPages { get; init; } = new();

        public MockDbContextBuilder() { }

        public Mock<IWAContext> Build()
        {
            var mockContext = new Mock<IWAContext>();

            mockContext.Setup(c => c.Users).Returns(CreateMockDbSet(Users));
            mockContext.Setup(c => c.Appointments).Returns(CreateMockDbSet(Appointments));
            mockContext.Setup(c => c.Categories).Returns(CreateMockDbSet(Categories));
            mockContext.Setup(c => c.ContractorPages).Returns(CreateMockDbSet(ContractorPages));

            mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            return mockContext;
        }

        private static DbSet<T> CreateMockDbSet<T>(List<T> source)
            where T : class
        {
            var queryableData = source.AsQueryable();
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryableData.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());
            mockSet.Setup(m => m.Add(It.IsAny<T>())).Callback<T>(x => source.Add(x));

            return mockSet.Object;
        }
    }
}
