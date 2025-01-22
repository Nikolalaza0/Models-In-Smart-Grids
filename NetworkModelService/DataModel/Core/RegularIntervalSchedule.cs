using FTN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
    public class RegularIntervalSchedule : BasicIntervalSchedule
    {
        private DateTime endTime;
        private float timeStep;
        private List<long> timePoints = new List<long>();


        public RegularIntervalSchedule(long globalId) : base(globalId)
        {
        }

        public DateTime EndTime { get => endTime; set => endTime = value; }
        public float TimeStep { get => timeStep; set => timeStep = value; }
        public List<long> TimePoints { get => timePoints; set => timePoints = value; }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                RegularIntervalSchedule x = (RegularIntervalSchedule)obj;
                return (
                        x.endTime == this.endTime
                     && x.timeStep == this.timeStep
                     && CompareHelper.CompareLists(x.timePoints, this.timePoints, true));
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

        public override bool HasProperty(ModelCode t)
        {
            switch (t)
            {
                case ModelCode.REGULARINTERVALSCH_ENDTIME:
                case ModelCode.REGULARINTERVALSCH_TIMESTEP:
                case ModelCode.REGULARINTERVALSCH_TIMEPOINTS:

                    return true;

                default:
                    return base.HasProperty(t);
            }
        }

        public override void GetProperty(Property prop)
        {
            switch (prop.Id)
            {
                case ModelCode.REGULARINTERVALSCH_ENDTIME:
                    prop.SetValue(endTime);
                    break;

                case ModelCode.REGULARINTERVALSCH_TIMESTEP:
                    prop.SetValue(timeStep);
                    break;
                case ModelCode.REGULARINTERVALSCH_TIMEPOINTS:
                    prop.SetValue(timePoints);
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
                case ModelCode.REGULARINTERVALSCH_ENDTIME:
                    endTime =property.AsDateTime();
                    break;
                case ModelCode.REGULARINTERVALSCH_TIMESTEP:
                    timeStep = property.AsFloat();
                    break;

                default:
                    base.SetProperty(property);
                    break;
            }
        }

        #endregion IAccess implementation


        #region IReference implementation

        public override bool IsReferenced
        {
            get
            {
                return timePoints.Count > 0 || base.IsReferenced;
            }
        }

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (timePoints != null && timePoints.Count > 0 && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
            {
                references[ModelCode.REGULARINTERVALSCH_TIMEPOINTS] = timePoints.GetRange(0, timePoints.Count);
            }

            base.GetReferences(references, refType);
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.REGULARTIMEPOINT_INTERVALSCH:
                    timePoints.Add(globalId);
                    break;

                default:
                    base.AddReference(referenceId, globalId);
                    break;
            }
        }

        public override void RemoveReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.REGULARTIMEPOINT_INTERVALSCH:

                    if (timePoints.Contains(globalId))
                    {
                        timePoints.Remove(globalId);
                    }
                    else
                    {
                        CommonTrace.WriteTrace(CommonTrace.TraceWarning, "Entity (GID = 0x{0:x16}) doesn't contain reference 0x{1:x16}.", this.GlobalId, globalId);
                    }

                    break;

                default:
                    base.RemoveReference(referenceId, globalId);
                    break;
            }
        }

        #endregion IReference implementation	

    }
}
