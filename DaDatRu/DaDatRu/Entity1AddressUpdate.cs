using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaDatRu
{
    public class Entity1AddressUpdate : IPlugin
    {
        //step -> pre-operation/ sync/ contains PreImage named ,,PreImage''
        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)
               serviceProvider.GetService(typeof(IPluginExecutionContext));
            var serviceFactory =
                (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            var service = serviceFactory.CreateOrganizationService(context.UserId);

            if (context.InputParameters.Contains("Target") &&
               context.InputParameters["Target"] is Entity entity)
            {
                try
                {
                    if (entity.LogicalName == "mcs_Entity1" && context.MessageName.ToLower() == "update")
                    {
                        Entity entityPreImage = null;
                        if (context.PreEntityImages.Contains("PreImage"))
                        {
                            entityPreImage = context.PreEntityImages["PreImage"];
                        }

                        var fullAddress = entity.Attributes.Contains("mcs_fulladdress") ? entity.GetAttributeValue<string>("mcs_fulladdress") : entityPreImage.GetAttributeValue<string>("mcs_fulladdress");

                        if (fullAddress != null && fullAddress != string.Empty)
                        {
                            var daDataAddressObj = GetAddressObjByFullAddress(fullAddress);

                            entity["mcs_country"] = daDataAddressObj.country;
                            entity["mcs_region"] = daDataAddressObj.region_type_full;
                            entity["mcs_regionid"] = daDataAddressObj.region_fias_id;
                            entity["mcs_city"] = daDataAddressObj.city_district;
                            entity["mcs_cityid"] = daDataAddressObj.city_district_fias_id;
                            entity["mcs_street"] = daDataAddressObj.street;
                            entity["mcs_streetid"] = daDataAddressObj.street_fias_id;
                            entity["mcs_house"] = daDataAddressObj.house;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("DaDataRu.Entity1AddressUpdate.Execute threw an exception", ex);
                }
            }
        }
        private Dadata.Model.Address GetAddressObjByFullAddress(string fullAddress)
        {
            try
            {
                var token = ConfigurationManager.AppSettings["Token"];
                var secret = ConfigurationManager.AppSettings["Secret"];
                var api = new DaDataRuRepository.DaDataRuAddress(token, secret);

                var addressApiResult = api.GetAddress(fullAddress);

                return addressApiResult;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("DaDataRu.Entity1AddressUpdate.GetAddressObjByFullAddress threw and exception", ex);
            }
        }
    }
}

