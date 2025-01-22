using FTN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
    public class IrregularTimePoint : IdentifiedObject
    {
        private float time;
        private float value1;
        private float value2;
        private long intervalSchedule = 0;

        public IrregularTimePoint(long globalId) : base(globalId)
        {
        }

        public float Time { get => time; set => time = value; }
        public float Value1 { get => value1; set => value1 = value; }
        public float Value2 { get => value2; set => value2 = value; }
        public long IntervalSchedule { get => intervalSchedule; set => intervalSchedule = value; }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                IrregularTimePoint x = (IrregularTimePoint)obj;
                return (x.time == this.time
                     && x.value1 == this.value1
                     && x.value2 == this.value2
                     && x.intervalSchedule == this.intervalSchedule);
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
                case ModelCode.IRREGULARTP_TIME:
                case ModelCode.IRREGULARTP_VALUE1:
                case ModelCode.IRREGULARTP_VALUE2:
                case ModelCode.IRREGULARTP_INTERVALSCH:
                    return true;

                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property prop)
        {
            switch (prop.Id)
            {
                case ModelCode.IRREGULARTP_TIME:
                    prop.SetValue(time);
                    break;

                case ModelCode.IRREGULARTP_VALUE1:
                    prop.SetValue(value1);
                    break;

                case ModelCode.IRREGULARTP_VALUE2:
                    prop.SetValue(value2);
                    break;

                case ModelCode.IRREGULARTP_INTERVALSCH:
                    prop.SetValue(intervalSchedule);
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
                case ModelCode.IRREGULARTP_TIME:
                    time = property.AsFloat();
                    break;
                case ModelCode.IRREGULARTP_VALUE1:
                    value1 = property.AsFloat();
                    break;
                case ModelCode.IRREGULARTP_VALUE2:
                    value2 = property.AsFloat();
                    break;
                case ModelCode.IRREGULARTP_INTERVALSCH:
                    intervalSchedule = property.AsReference();
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
            if (intervalSchedule != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
            {
                references[ModelCode.IRREGULARTP_INTERVALSCH] = new List<long>();
                references[ModelCode.IRREGULARTP_INTERVALSCH].Add(intervalSchedule);
            }

            base.GetReferences(references, refType);
        }

        #endregion IReference implementation	

    }
}
