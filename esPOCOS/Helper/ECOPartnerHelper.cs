using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{
    public class ECOPartnerHelper : Helper
    {

        public ECOPartner getECOPartnerById(int id)
        {
            if (id == 0)
                return null;
            try
            {
                var _ecoPartner = (from p in context.ECOPartners
                                   where p.PartnerId == id
                                   select p).FirstOrDefault();
                if (_ecoPartner != null)
                    _ecoPartner.helper = this;
                return _ecoPartner;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }

        public List<ECOPartner> getECOPartnerByBaseInfor(List<string> specialties,string country,string state,string industry)
        {
            if (specialties.Count == 0 && string.IsNullOrEmpty(country) && string.IsNullOrEmpty(state) && string.IsNullOrEmpty(industry))
                return new List<ECOPartner>();

            List<ECOPartner> resultLS = new List<ECOPartner>();
            var lsx = (from ep in context.ECOPartners
                       join esc in context.ECOPartnerServiceCoverages on ep.PartnerId equals esc.PartnerId
                       where ep.Status.Equals("Passing") && 
                            (string.IsNullOrEmpty(country) ? true : esc.CountryName == country)
                            && (string.IsNullOrEmpty(state) ? string.IsNullOrEmpty(esc.StateName) : string.IsNullOrEmpty(esc.StateName) || esc.StateName == state)
                       select ep).Distinct().ToList();
            if (!string.IsNullOrEmpty(industry))
                lsx = (from c in lsx
                       where c.ECOPartnerIndustries.Select(pi => pi.ECOIndustry.IndustryName).Contains(industry)
                       select c).ToList();
            foreach (ECOPartner partner in lsx)
            {
                foreach(var sp in partner.ECOPartnerSpecialties)
                {
                    if (specialties.Any(x => string.IsNullOrEmpty(x) == false)==false || specialties.Where(x => string.IsNullOrEmpty(x) == false).Contains(sp.ECOSpecialty.SpecialtyName))
                    {
                        partner.helper = this;
                        resultLS.Add(partner);
                        break;
                    }
                }
            }
            return resultLS;
        }

        public List<ECOPartner> getAllECOPartner(int specialtyId = 0, int industryId = 0, string country = "", string countryState = "", string companyName = "", string status = "")
        {
            List<ECOPartner> ecoPartnerList = new List<ECOPartner>();
            if (specialtyId == 0 && industryId == 0 && string.IsNullOrEmpty(country) && string.IsNullOrEmpty(countryState))
            {
                ecoPartnerList = (from ep in context.ECOPartners
                                  where (string.IsNullOrEmpty(companyName) ? true : ep.CompanyName.Contains(companyName))
                                  && (string.IsNullOrEmpty(status) ? true : ep.Status == status)
                                  select ep).Distinct().ToList();
            }
            else
            {
                //left join
                ecoPartnerList = (from ep in context.ECOPartners
                                  join esc in context.ECOPartnerServiceCoverages on ep.PartnerId equals esc.PartnerId into escItem
                                  from escData in escItem.DefaultIfEmpty()
                                  join eps in context.ECOPartnerSpecialties on ep.PartnerId equals eps.PartnerId into epsItem
                                  from epsData in epsItem.DefaultIfEmpty()
                                  join epi in context.ECOPartnerIndustries on ep.PartnerId equals epi.PartnerId into epiItem
                                  from epiData in epiItem.DefaultIfEmpty()
                                  where (specialtyId == 0 ? true : epsData.SpecialtyId == specialtyId)
                                  && (industryId == 0 ? true : epiData.IndustryId == industryId)
                                  && (string.IsNullOrEmpty(country) ? true : escData.CountryName == country)
                                  && (string.IsNullOrEmpty(countryState) ? true : escData.StateName == countryState)
                                  && (string.IsNullOrEmpty(companyName) ? true : ep.CompanyName.Contains(companyName))
                                  && (string.IsNullOrEmpty(status) ? true : ep.Status == status)
                                  select ep).Distinct().ToList();

                //right join 交叉集
                //ecoPartnerList = (from ep in context.ECOPartners
                //                  from esc in context.ECOPartnerServiceCoverages
                //                  from eps in context.ECOPartnerSpecialties
                //                  from epi in context.ECOPartnerIndustries
                //                  where (specialtyId == 0 ? true : ep.PartnerId == eps.PartnerId && eps.SpecialtyId == specialtyId)
                //                  && (industryId == 0 ? true : ep.PartnerId == epi.PartnerId && epi.IndustryId == industryId)
                //                  && (string.IsNullOrEmpty(country) ? true : ep.PartnerId == esc.PartnerId && esc.CountryName == country)
                //                  && (string.IsNullOrEmpty(countryState) ? true : ep.PartnerId == esc.PartnerId && esc.StateName == countryState)
                //                  && (string.IsNullOrEmpty(companyName) ? true : ep.CompanyName.Contains(companyName))
                //                  && (string.IsNullOrEmpty(status) ? true : ep.Status == status)
                //                  select ep).Distinct().ToList();
            }
            if (ecoPartnerList != null)
            {
                foreach (var item in ecoPartnerList)
	            {
                    item.helper = this;
	            }
            }
            return ecoPartnerList;
        }

        public int save(ECOPartner eCOPartner)
        {
            if (eCOPartner == null || !eCOPartner.validate())
                return 1;
            try
            {
                var _exitobj = getECOPartnerById(eCOPartner.PartnerId);
                if (_exitobj != null)
                    context.ECOPartners.ApplyCurrentValues(eCOPartner);
                else
                    context.ECOPartners.AddObject(eCOPartner);
                context.SaveChanges();
                return 0;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return 500;
            }
        }

        public int delete(ECOPartner eCOPartner)
        {
            if (eCOPartner == null)
                return 1;
            try
            {
                context.ECOPartners.DeleteObject(eCOPartner);
                context.SaveChanges();
                return 0;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return 500;
            }
        }
    }
}
