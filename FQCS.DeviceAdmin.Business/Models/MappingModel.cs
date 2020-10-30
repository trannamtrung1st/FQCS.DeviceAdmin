using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace FQCS.DeviceAdmin.Business.Models
{
    public partial interface IMappingModel
    {
        void CopyFrom(object src, IMapper mapper = null);
        void CopyTo(object dest, IMapper mapper = null);
        ET To<ET>(IMapper mapper = null);
    }

    public partial interface IMappingModel<E> : IMappingModel
    {
        void FromSrc(E src, IMapper mapper = null);
        E ToDest(IMapper mapper = null);
    }

    public abstract partial class MappingModel<E> : IMappingModel<E>
    {
        protected readonly IMapper defaultMapper;

        public MappingModel()
        {
            this.defaultMapper = Global.Mapper;
        }

        public MappingModel(E src) : this()
        {
            FromSrc(src);
        }

        protected E Src { get; set; }

        public virtual void FromSrc(E src, IMapper mapper = null)
        {
            mapper = mapper ?? defaultMapper;
            Src = src;
            mapper.Map(src, this);
        }

        public virtual void CopyFrom(object src, IMapper mapper = null)
        {
            mapper = mapper ?? defaultMapper;
            mapper.Map(src, this);
        }

        public virtual void CopyTo(object dest, IMapper mapper = null)
        {
            mapper = mapper ?? defaultMapper;
            mapper.Map(this, dest);
        }

        public virtual ET To<ET>(IMapper mapper = null)
        {
            mapper = mapper ?? defaultMapper;
            return mapper.Map<ET>(this);
        }

        public virtual E ToDest(IMapper mapper = null)
        {
            mapper = mapper ?? defaultMapper;
            if (Src != null)
                return Src;
            return mapper.Map<E>(this);
        }

        public virtual E ToDest(bool copyToSrc, IMapper mapper = null)
        {
            mapper = mapper ?? defaultMapper;
            if (Src != null)
            {
                if (copyToSrc) CopyTo(Src);
                return Src;
            }
            return mapper.Map<E>(this);
        }

    }
}
