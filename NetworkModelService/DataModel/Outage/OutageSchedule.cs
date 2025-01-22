using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTN.Services.NetworkModelService.DataModel.Outage
{
    public class OutageSchedule : IrregularIntervalSchedule
    {
        private long powerSystemResource = 0;

        public OutageSchedule(long globalId) : base(globalId)
        {
        }

        public long PowerSystemResource { get => powerSystemResource; set => powerSystemResource = value; }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                OutageSchedule x = (OutageSchedule)obj;
                return (x.powerSystemResource == this.powerSystemResource);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region IAccess implementation
        public override bool HasProperty(ModelCode property)
        {
            switch (property)
            {
                case ModelCode.OUTAGESCH_PSR:
                    return true;

                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property prop)
        {
            switch (prop.Id)
            {
                case ModelCode.OUTAGESCH_PSR:
                    prop.SetValue(powerSystemResource);
                    break;
                default:
                    base.GetProperty(prop);
                    break;
            }
        }

        public override void SetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.OUTAGESCH_PSR:
                    powerSystemResource = property.AsReference();
                    break;

                default:
                    base.SetProperty(property);
                    break;
            }
        }

        #endregion IAccess implementation

        #region IReference implementation

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (powerSystemResource != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
            {
                references[ModelCode.OUTAGESCH_PSR] = new List<long>();
                references[ModelCode.OUTAGESCH_PSR].Add(powerSystemResource);
            }

            base.GetReferences(references, refType);
        }

        #endregion IReference implementation	
    }
}
