using System.Threading.Tasks;
using Digipolis.DataAccess;
using Microsoft.AspNetCore.Mvc;
using StarterKit.Entities;

namespace StarterKit.Api.Controllers
{
    [Route("[controller]")]
    public class DbExamplesController : Controller
    {
        private readonly IUowProvider _uowProvider;

        public DbExamplesController(IUowProvider uowProvider)
        {
            _uowProvider = uowProvider;
        }

        [HttpPost]
        public async Task<int> Post(MyEntity entity)
        {
            using var uow = _uowProvider.CreateUnitOfWork();
            var repository = uow.GetRepository<MyEntity>();

            repository.Add(entity);

            return await uow.SaveChangesAsync();
        }

    }
}
