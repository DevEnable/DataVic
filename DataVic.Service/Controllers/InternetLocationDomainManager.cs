using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.OData;
using AutoMapper;
using DataVic.Service.DataObjects;
using DataVic.Service.Models;
using Microsoft.WindowsAzure.Mobile.Service;

namespace DataVic.Service.Controllers
{
    public class InternetLocationDomainManager : MappedEntityDomainManager<InternetLocationModel, InternetLocation>
    {
        public InternetLocationDomainManager(DataVicContext context, HttpRequestMessage request, ApiServices services)
            : base(context, request, services)
        {
        }

        public override SingleResult<InternetLocationModel> Lookup(string id)
        {
            var key = GetKey<string>(id);
            InternetLocation location = this.Context.Set<InternetLocation>().Find(key);
            List<InternetLocationModel> result = new List<InternetLocationModel>();

            if (location != null)
            {
                result.Add(Mapper.Map<InternetLocationModel>(location));
            }

            return SingleResult.Create(result.AsQueryable());
        }

        public async override Task<InternetLocationModel> UpdateAsync(string id, Delta<InternetLocationModel> patch)
        {
            var key = GetKey<string>(id);
            InternetLocation model = await this.Context.Set<InternetLocation>().FindAsync(key);
            if (model == null)
            {
                throw new HttpResponseException(this.Request.CreateNotFoundResponse());
            }

            InternetLocationModel data = Mapper.Map<InternetLocationModel>(model);

            patch.Patch(data);

            // Need to update reference types too.
            // TODO: Update Geo-Location
            foreach (var pn in patch.GetChangedPropertyNames())
            {
                Type t;
                if (patch.TryGetPropertyType(pn, out t) && t.IsClass)
                {
                    object v;
                    if (patch.TryGetPropertyValue(pn, out v))
                    {
                        data.GetType().GetProperty(pn).GetSetMethod().Invoke(data, new[] { v });
                    }
                }
            }

            Mapper.Map(data, model);
            await this.SubmitChangesAsync();

            return data;
        }

        public override Task<bool> DeleteAsync(string id)
        {
            var key = GetKey<string>(id);
            return this.DeleteItemAsync(key);
        }
        
        protected override SingleResult<InternetLocationModel> LookupEntity(Expression<Func<InternetLocation, bool>> filter)
        {
            DataVicContext context = (DataVicContext) this.Context;


            return SingleResult.Create(context.InternetLocations.Where(filter)
                .Select(Mapper.Map<InternetLocationModel>).AsQueryable());
        }

    }
}