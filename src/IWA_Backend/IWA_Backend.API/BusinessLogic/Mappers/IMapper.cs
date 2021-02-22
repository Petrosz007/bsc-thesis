using IWA_Backend.API.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IWA_Backend.API.BusinessLogic.Mappers
{
    public interface IMapper<TEntity, TDTO>
    {
        public TEntity ToEntity(TDTO dto);
        public TDTO ToDTO(TEntity entity);
    }
}
