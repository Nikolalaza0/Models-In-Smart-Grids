using FTN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
    public class RegularTimePoint : IdentifiedObject
    {
        private int sequenceNumber;
        private float value1;
        private float value2;
        private long intervalSchedule = 0;


        public RegularTimePoint(long globalId) : base(globalId)
        {
        }

        public int SequenceNumber { get => sequenceNumber; set => sequenceNumber = value; }
        public float Value1 { get => value1; set => value1 = value; }
        public float Value2 { get => value2; set => value2 = value; }
        public long IntervalSchedule { get => intervalSchedule; set => intervalSchedule = value; }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                RegularTimePoint x = (RegularTimePoint)obj;
                return (x.sequenceNumber == this.sequenceNumber 
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
                case ModelCode.REGULARTIMEPOINT_SEQNUM:
                case ModelCode.REGULARTIMEPOINT_VALUE1:
                case ModelCode.REGULARTIMEPOINT_VALUE2:
                case ModelCode.REGULARTIMEPOINT_INTERVALSCH:
                    return true;

                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property prop)
        {
            switch (prop.Id)
            {
                case ModelCode.REGULARTIMEPOINT_SEQNUM:
                    prop.SetValue(sequenceNumber);
                    break;

                case ModelCode.REGULARTIMEPOINT_VALUE1:
                    prop.SetValue(value1);
                    break;

                case ModelCode.REGULARTIMEPOINT_VALUE2:
                    prop.SetValue(value2);
                    break;

                case ModelCode.REGULARTIMEPOINT_INTERVALSCH:
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
                case ModelCode.REGULARTIMEPOINT_SEQNUM:
                    sequenceNumber = property.AsInt();
                    break;
                case ModelCode.REGULARTIMEPOINT_VALUE1:
                    value1 = property.AsFloat();
                    break;
                case ModelCode.REGULARTIMEPOINT_VALUE2:
                    value2 = property.AsFloat();
                    break;
                case ModelCode.REGULARTIMEPOINT_INTERVALSCH:
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
                references[ModelCode.REGULARTIMEPOINT_INTERVALSCH] = new List<long>();
                references[ModelCode.REGULARTIMEPOINT_INTERVALSCH].Add(intervalSchedule);
            }

            base.GetReferences(references, refType);
        }

        #endregion IReference implementation	

    }
}
