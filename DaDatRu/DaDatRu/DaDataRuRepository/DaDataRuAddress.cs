using Dadata.Model;
using DaDatRu.DaDataRuGeneral;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace DaDatRu.DaDataRuRepository
{
    class DaDataRuAddress : DaDataRuContext
    {
        private const string AddressUrl = "https://cleaner.dadata.ru/api/v1/clean";

        internal DaDataRuAddress(string Token, string Secret, string Url = AddressUrl) : base(Token,Secret,Url)
        {
            serializer.Converters.Add(new StringEnumConverter());
        }

        internal Address GetAddress(string data)
        {
            try
            {
                var structure = new List<Dadata.Model.StructureType>(
                new Dadata.Model.StructureType[] {Dadata.Model.StructureType.ADDRESS});
                var dataToSend = new string[] { data };
                var request = Clean(structure, dataToSend);

                return (Address)request[0];
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException("DaDataRu.DaDataRuAddress.GetAddress threw and exception", ex);
            }
        }
    }
}
