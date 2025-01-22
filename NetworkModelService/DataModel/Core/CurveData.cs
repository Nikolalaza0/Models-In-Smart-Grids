using FTN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
    public class CurveData : IdentifiedObject
    {
        private float xValue;
        private float y1Value;
        private float y2Value;
        private float y3Value;
        private long curve = 0;

        public CurveData(long globalId) : base(globalId)
        {
        }


        public float XValue { get => xValue; set => xValue = value; }
        public float Y1Value { get => y1Value; set => y1Value = value; }
        public float Y2Value { get => y2Value; set => y2Value = value; }
        public float Y3Value { get => y3Value; set => y3Value = value; }
        public long Curve { get => curve; set => curve = value; }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                CurveData x = (CurveData)obj;
                return (x.xValue == this.xValue && x.y1Value == this.y1Value && x.y2Value == this.y2Value && x.y3Value == this.y3Value && x.curve == this.curve);
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
                case ModelCode.CURVEDATA_XVALUE:
                case ModelCode.CURVEDATA_Y1VALUE:
                case ModelCode.CURVEDATA_Y2VALUE:
                case ModelCode.CURVEDATA_Y3VALUE:
                case ModelCode.CURVEDATA_CURVE:
                    return true;

                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property prop)
        {
            switch (prop.Id)
            {
                case ModelCode.CURVEDATA_XVALUE:
                    prop.SetValue(xValue);
                    break;

                case ModelCode.CURVEDATA_Y1VALUE:
                    prop.SetValue(y1Value);
                    break;

                case ModelCode.CURVEDATA_Y2VALUE:
                    prop.SetValue(y2Value);
                    break;

                case ModelCode.CURVEDATA_Y3VALUE:
                    prop.SetValue(y3Value);
                    break;

                case ModelCode.CURVEDATA_CURVE:
                    prop.SetValue(curve);
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
                case ModelCode.CURVEDATA_XVALUE:
                    xValue = property.AsFloat();
                    break;
                case ModelCode.CURVEDATA_Y1VALUE:
                    y1Value = property.AsFloat();
                    break;
                case ModelCode.CURVEDATA_Y2VALUE:
                    y2Value = property.AsFloat();
                    break;
                case ModelCode.CURVEDATA_Y3VALUE:
                    y3Value = property.AsFloat();
                    break;
                case ModelCode.CURVEDATA_CURVE:
                    y3Value = property.AsReference();
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
            if (curve != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
            {
                references[ModelCode.CURVEDATA_CURVE] = new List<long>();
                references[ModelCode.CURVEDATA_CURVE].Add(curve);
            }

            base.GetReferences(references, refType);
        }

        #endregion IReference implementation		
    }
}
