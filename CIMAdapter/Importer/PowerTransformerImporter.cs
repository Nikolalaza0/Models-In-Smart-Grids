using System;
using System.Collections.Generic;
using CIM.Model;
using FTN.Common;
using FTN.ESI.SIMES.CIM.CIMAdapter.Manager;

namespace FTN.ESI.SIMES.CIM.CIMAdapter.Importer
{
	/// <summary>
	/// PowerTransformerImporter
	/// </summary>
	public class PowerTransformerImporter
	{
		/// <summary> Singleton </summary>
		private static PowerTransformerImporter ptImporter = null;
		private static object singletoneLock = new object();

		private ConcreteModel concreteModel;
		private Delta delta;
		private ImportHelper importHelper;
		private TransformAndLoadReport report;


		#region Properties
		public static PowerTransformerImporter Instance
		{
			get
			{
				if (ptImporter == null)
				{
					lock (singletoneLock)
					{
						if (ptImporter == null)
						{
							ptImporter = new PowerTransformerImporter();
							ptImporter.Reset();
						}
					}
				}
				return ptImporter;
			}
		}

		public Delta NMSDelta
		{
			get 
			{
				return delta;
			}
		}
		#endregion Properties


		public void Reset()
		{
			concreteModel = null;
			delta = new Delta();
			importHelper = new ImportHelper();
			report = null;
		}

		public TransformAndLoadReport CreateNMSDelta(ConcreteModel cimConcreteModel)
		{
			LogManager.Log("Importing PowerTransformer Elements...", LogLevel.Info);
			report = new TransformAndLoadReport();
			concreteModel = cimConcreteModel;
			delta.ClearDeltaOperations();

			if ((concreteModel != null) && (concreteModel.ModelMap != null))
			{
				try
				{
					// convert into DMS elements
					ConvertModelAndPopulateDelta();
				}
				catch (Exception ex)
				{
					string message = string.Format("{0} - ERROR in data import - {1}", DateTime.Now, ex.Message);
					LogManager.Log(message);
					report.Report.AppendLine(ex.Message);
					report.Success = false;
				}
			}
			LogManager.Log("Importing PowerTransformer Elements - END.", LogLevel.Info);
			return report;
		}

        /// <summary>
        /// Method performs conversion of network elements from CIM based concrete model into DMS model.
        /// </summary>
        /// 

        //PROTECTEDSWITCH						= 0x0001,
        //DISCONNECTOR						= 0x0002,
        //IRREGULARTP                         = 0x0003,
        //OUTAGESCH                           = 0x0004,
        //CURVE								= 0x0005,
        //CURVEDATA							= 0x0006,
        //REGULARTIMEPOINT					= 0x0007,
        //REGULARINTERVALSCH  				= 0x0008,
		private void ConvertModelAndPopulateDelta()
		{
			LogManager.Log("Loading elements and creating delta...", LogLevel.Info);

			//// import all concrete model types (DMSType enum)
			ImportProtectedSwitch();
            ImportDisconnector();
            ImportOutageSchedule();
            ImportIrregularTimePoint();
            ImportCurve();
            ImportCurveData();
            ImportRegularIntervalSchedule();
            ImportRegularTimePoint();





			LogManager.Log("Loading elements and creating delta completed.", LogLevel.Info);
		}

        #region myImports
        private void ImportProtectedSwitch()
        {
            SortedDictionary<string, object> cimProtectedSwitches = concreteModel.GetAllObjectsOfType("FTN.ProtectedSwitch");
            if (cimProtectedSwitches != null)
            {
                foreach (KeyValuePair<string, object> cimProtectedSwitchesPair in cimProtectedSwitches)
                {
                    FTN.ProtectedSwitch cimProtectedSwitch = cimProtectedSwitchesPair.Value as FTN.ProtectedSwitch;

                    ResourceDescription rd = CreateProtectedSwitchResourceDescription(cimProtectedSwitch);
                    if (rd != null)
                    {
                        delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                        report.Report.Append("ProtectedSwitch ID = ").Append(cimProtectedSwitch.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
                    }
                    else
                    {
                        report.Report.Append("ProtectedSwitch ID = ").Append(cimProtectedSwitch.ID).AppendLine(" FAILED to be converted");
                    }
                }
                report.Report.AppendLine();
            }
        }

        private ResourceDescription CreateProtectedSwitchResourceDescription(FTN.ProtectedSwitch cimProtectedSwitch)
        {
            ResourceDescription rd = null;
            if (cimProtectedSwitch != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.PROTECTEDSWITCH, importHelper.CheckOutIndexForDMSType(DMSType.PROTECTEDSWITCH));
                rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimProtectedSwitch.ID, gid);

                ////populate ResourceDescription
                PowerTransformerConverter.PopulateProtectedSwitchProperties(cimProtectedSwitch, rd, importHelper, report);
            }
            return rd;
        }

        private void ImportDisconnector()
        {
            SortedDictionary<string, object> cimDisconnectors = concreteModel.GetAllObjectsOfType("FTN.Disconnector");
            if (cimDisconnectors != null)
            {
                foreach (KeyValuePair<string, object> cimDisconnectorPair in cimDisconnectors)
                {
                    FTN.Disconnector cimDisconnector = cimDisconnectorPair.Value as FTN.Disconnector;

                    ResourceDescription rd = CreateDisconnectorResourceDescription(cimDisconnector);
                    if (rd != null)
                    {
                        delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                        report.Report.Append("Disconnector ID = ").Append(cimDisconnector.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
                    }
                    else
                    {
                        report.Report.Append("Disconnector ID = ").Append(cimDisconnector.ID).AppendLine(" FAILED to be converted");
                    }
                }
                report.Report.AppendLine();
            }
        }

        private ResourceDescription CreateDisconnectorResourceDescription(FTN.Disconnector cimDisconnector)
        {
            ResourceDescription rd = null;
            if (cimDisconnector != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.DISCONNECTOR, importHelper.CheckOutIndexForDMSType(DMSType.DISCONNECTOR));
                rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimDisconnector.ID, gid);

                ////populate ResourceDescription
                PowerTransformerConverter.PopulateDisconnectorProperties(cimDisconnector, rd, importHelper, report);
            }
            return rd;
        }


        private void ImportIrregularTimePoint()
        {
            SortedDictionary<string, object> cimIrregularTimePoints = concreteModel.GetAllObjectsOfType("FTN.IrregularTimePoint");
            if (cimIrregularTimePoints != null)
            {
                foreach (KeyValuePair<string, object> cimIrregularTimePointPair in cimIrregularTimePoints)
                {
                    FTN.IrregularTimePoint cimIrregularTimePoint = cimIrregularTimePointPair.Value as FTN.IrregularTimePoint;

                    ResourceDescription rd = CreateIrregularTimePointhResourceDescription(cimIrregularTimePoint);
                    if (rd != null)
                    {
                        delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                        report.Report.Append("IrregularTimePoint ID = ").Append(cimIrregularTimePoint.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
                    }
                    else
                    {
                        report.Report.Append("IrregularTimePoint ID = ").Append(cimIrregularTimePoint.ID).AppendLine(" FAILED to be converted");
                    }
                }
                report.Report.AppendLine();
            }
        }

        private ResourceDescription CreateIrregularTimePointhResourceDescription(FTN.IrregularTimePoint cimIrregularTimePoint)
        {
            ResourceDescription rd = null;
            if (cimIrregularTimePoint != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.IRREGULARTP, importHelper.CheckOutIndexForDMSType(DMSType.IRREGULARTP));
                rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimIrregularTimePoint.ID, gid);

                ////populate ResourceDescription
                PowerTransformerConverter.PopulateIrregularTimePointProperties(cimIrregularTimePoint, rd, importHelper, report);
            }
            return rd;
        }

        private void ImportOutageSchedule()
        {
            SortedDictionary<string, object> cimOutageSchedules = concreteModel.GetAllObjectsOfType("FTN.OutageSchedule");
            if (cimOutageSchedules != null)
            {
                foreach (KeyValuePair<string, object> cimOutageSchedulesPair in cimOutageSchedules)
                {
                    FTN.OutageSchedule cimOutageSchedule = cimOutageSchedulesPair.Value as FTN.OutageSchedule;

                    ResourceDescription rd = CreateOutageScheduleResourceDescription(cimOutageSchedule);
                    if (rd != null)
                    {
                        delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                        report.Report.Append("OutageSchedule ID = ").Append(cimOutageSchedule.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
                    }
                    else
                    {
                        report.Report.Append("OutageSchedule ID = ").Append(cimOutageSchedule.ID).AppendLine(" FAILED to be converted");
                    }
                }
                report.Report.AppendLine();
            }
        }

        private ResourceDescription CreateOutageScheduleResourceDescription(FTN.OutageSchedule cimOutageSchedule)
        {
            ResourceDescription rd = null;
            if (cimOutageSchedule != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.OUTAGESCH, importHelper.CheckOutIndexForDMSType(DMSType.OUTAGESCH));
                rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimOutageSchedule.ID, gid);

                ////populate ResourceDescription
                PowerTransformerConverter.PopulateOutageScheduleProperties(cimOutageSchedule, rd, importHelper, report);
            }
            return rd;
        }

        private void ImportCurve()
        {
            SortedDictionary<string, object> cimCurves = concreteModel.GetAllObjectsOfType("FTN.Curve");
            if (cimCurves != null)
            {
                foreach (KeyValuePair<string, object> cimCurvespair in cimCurves)
                {
                    FTN.Curve cimCurve = cimCurvespair.Value as FTN.Curve;

                    ResourceDescription rd = CreateCurveResourceDescription(cimCurve);
                    if (rd != null)
                    {
                        delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                        report.Report.Append("Curve ID = ").Append(cimCurve.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
                    }
                    else
                    {
                        report.Report.Append("Curve ID = ").Append(cimCurve.ID).AppendLine(" FAILED to be converted");
                    }
                }
                report.Report.AppendLine();
            }
        }

        private ResourceDescription CreateCurveResourceDescription(FTN.Curve cimCurve)
        {
            ResourceDescription rd = null;
            if (cimCurve != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.CURVE, importHelper.CheckOutIndexForDMSType(DMSType.CURVE));
                rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimCurve.ID, gid);

                ////populate ResourceDescription
                PowerTransformerConverter.PopulateCurveProperties(cimCurve, rd, importHelper, report);
            }
            return rd;
        }

        private void ImportCurveData()
        {
            SortedDictionary<string, object> cimCurveDatas = concreteModel.GetAllObjectsOfType("FTN.CurveData");
            if (cimCurveDatas != null)
            {
                foreach (KeyValuePair<string, object> cimCurveDatasPair in cimCurveDatas)
                {
                    FTN.CurveData cimCurveData = cimCurveDatasPair.Value as FTN.CurveData;

                    ResourceDescription rd = CreateCurveDataResourceDescription(cimCurveData);
                    if (rd != null)
                    {
                        delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                        report.Report.Append("CurveData ID = ").Append(cimCurveData.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
                    }
                    else
                    {
                        report.Report.Append("CurveData ID = ").Append(cimCurveData.ID).AppendLine(" FAILED to be converted");
                    }
                }
                report.Report.AppendLine();
            }
        }

        private ResourceDescription CreateCurveDataResourceDescription(FTN.CurveData cimCurveData)
        {
            ResourceDescription rd = null;
            if (cimCurveData != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.CURVEDATA, importHelper.CheckOutIndexForDMSType(DMSType.CURVEDATA));
                rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimCurveData.ID, gid);

                ////populate ResourceDescription
                PowerTransformerConverter.PopulateCurveDataProperties(cimCurveData, rd, importHelper, report);
            }
            return rd;
        }

        private void ImportRegularTimePoint()
        {
            SortedDictionary<string, object> cimRegularTimePoints = concreteModel.GetAllObjectsOfType("FTN.RegularTimePoint");
            if (cimRegularTimePoints != null)
            {
                foreach (KeyValuePair<string, object> cimRegularTimePointsPair in cimRegularTimePoints)
                {
                    FTN.RegularTimePoint cimRegularTimePoint = cimRegularTimePointsPair.Value as FTN.RegularTimePoint;

                    ResourceDescription rd = CreateRegularTimePointResourceDescription(cimRegularTimePoint);
                    if (rd != null)
                    {
                        delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                        report.Report.Append("RegularTimePoint ID = ").Append(cimRegularTimePoint.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
                    }
                    else
                    {
                        report.Report.Append("RegularTimePoint ID = ").Append(cimRegularTimePoint.ID).AppendLine(" FAILED to be converted");
                    }
                }
                report.Report.AppendLine();
            }
        }

        private ResourceDescription CreateRegularTimePointResourceDescription(FTN.RegularTimePoint cimRegularTimePoint)
        {
            ResourceDescription rd = null;
            if (cimRegularTimePoint != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.REGULARTIMEPOINT, importHelper.CheckOutIndexForDMSType(DMSType.REGULARTIMEPOINT));
                rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimRegularTimePoint.ID, gid);

                ////populate ResourceDescription
                PowerTransformerConverter.PopulateRegularTimePointProperties(cimRegularTimePoint, rd, importHelper, report);
            }
            return rd;
        }

        private void ImportRegularIntervalSchedule()
        {
            SortedDictionary<string, object> cimRegularIntervalSchedules = concreteModel.GetAllObjectsOfType("FTN.RegularIntervalSchedule");
            if (cimRegularIntervalSchedules != null)
            {
                foreach (KeyValuePair<string, object> cimRegularIntervalSchedulesPair in cimRegularIntervalSchedules)
                {
                    FTN.RegularIntervalSchedule cimRegularIntervalSchedule = cimRegularIntervalSchedulesPair.Value as FTN.RegularIntervalSchedule;

                    ResourceDescription rd = CreateRegularIntervalScheduleResourceDescription(cimRegularIntervalSchedule);
                    if (rd != null)
                    {
                        delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                        report.Report.Append("RegularIntervalSchedule ID = ").Append(cimRegularIntervalSchedule.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
                    }
                    else
                    {
                        report.Report.Append("RegularIntervalSchedule ID = ").Append(cimRegularIntervalSchedule.ID).AppendLine(" FAILED to be converted");
                    }
                }
                report.Report.AppendLine();
            }
        }

        private ResourceDescription CreateRegularIntervalScheduleResourceDescription(FTN.RegularIntervalSchedule cimRegularIntervalSchedule)
        {
            ResourceDescription rd = null;
            if (cimRegularIntervalSchedule != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.REGULARINTERVALSCH, importHelper.CheckOutIndexForDMSType(DMSType.REGULARINTERVALSCH));
                rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimRegularIntervalSchedule.ID, gid);

                ////populate ResourceDescription
                PowerTransformerConverter.PopulateRegularIntervalScheduleProperties(cimRegularIntervalSchedule, rd, importHelper, report);
            }
            return rd;
        }

        #endregion



        #region Import
        //      private void ImportBaseVoltages()
        //{
        //	SortedDictionary<string, object> cimBaseVoltages = concreteModel.GetAllObjectsOfType("FTN.BaseVoltage");
        //	if (cimBaseVoltages != null)
        //	{
        //		foreach (KeyValuePair<string, object> cimBaseVoltagePair in cimBaseVoltages)
        //		{
        //			FTN.BaseVoltage cimBaseVoltage = cimBaseVoltagePair.Value as FTN.BaseVoltage;

        //			ResourceDescription rd = CreateBaseVoltageResourceDescription(cimBaseVoltage);
        //			if (rd != null)
        //			{
        //				delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
        //				report.Report.Append("BaseVoltage ID = ").Append(cimBaseVoltage.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
        //			}
        //			else
        //			{
        //				report.Report.Append("BaseVoltage ID = ").Append(cimBaseVoltage.ID).AppendLine(" FAILED to be converted");
        //			}
        //		}
        //		report.Report.AppendLine();
        //	}
        //}

        //private ResourceDescription CreateBaseVoltageResourceDescription(FTN.BaseVoltage cimBaseVoltage)
        //{
        //	ResourceDescription rd = null;
        //	if (cimBaseVoltage != null)
        //	{
        //		long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.BASEVOLTAGE, importHelper.CheckOutIndexForDMSType(DMSType.BASEVOLTAGE));
        //		rd = new ResourceDescription(gid);
        //		importHelper.DefineIDMapping(cimBaseVoltage.ID, gid);

        //		////populate ResourceDescription
        //		PowerTransformerConverter.PopulateBaseVoltageProperties(cimBaseVoltage, rd);
        //	}
        //	return rd;
        //}

        //private void ImportLocations()
        //{
        //	SortedDictionary<string, object> cimLocations = concreteModel.GetAllObjectsOfType("FTN.Location");
        //	if (cimLocations != null)
        //	{
        //		foreach (KeyValuePair<string, object> cimLocationPair in cimLocations)
        //		{
        //			FTN.Location cimLocation = cimLocationPair.Value as FTN.Location;

        //			ResourceDescription rd = CreateLocationResourceDescription(cimLocation);
        //			if (rd != null)
        //			{
        //				delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
        //				report.Report.Append("Location ID = ").Append(cimLocation.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
        //			}
        //			else
        //			{
        //				report.Report.Append("Location ID = ").Append(cimLocation.ID).AppendLine(" FAILED to be converted");
        //			}
        //		}
        //		report.Report.AppendLine();
        //	}
        //}

        //private ResourceDescription CreateLocationResourceDescription(FTN.Location cimLocation)
        //{
        //	ResourceDescription rd = null;
        //	if (cimLocation != null)
        //	{
        //		long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.LOCATION, importHelper.CheckOutIndexForDMSType(DMSType.LOCATION));
        //		rd = new ResourceDescription(gid);
        //		importHelper.DefineIDMapping(cimLocation.ID, gid);

        //		////populate ResourceDescription
        //		PowerTransformerConverter.PopulateLocationProperties(cimLocation, rd);
        //	}
        //	return rd;
        //}

        //private void ImportPowerTransformers()
        //{
        //	SortedDictionary<string, object> cimPowerTransformers = concreteModel.GetAllObjectsOfType("FTN.PowerTransformer");
        //	if (cimPowerTransformers != null)
        //	{
        //		foreach (KeyValuePair<string, object> cimPowerTransformerPair in cimPowerTransformers)
        //		{
        //			FTN.PowerTransformer cimPowerTransformer = cimPowerTransformerPair.Value as FTN.PowerTransformer;

        //			ResourceDescription rd = CreatePowerTransformerResourceDescription(cimPowerTransformer);
        //			if (rd != null)
        //			{
        //				delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
        //				report.Report.Append("PowerTransformer ID = ").Append(cimPowerTransformer.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
        //			}
        //			else
        //			{
        //				report.Report.Append("PowerTransformer ID = ").Append(cimPowerTransformer.ID).AppendLine(" FAILED to be converted");
        //			}
        //		}
        //		report.Report.AppendLine();
        //	}
        //}

        //private ResourceDescription CreatePowerTransformerResourceDescription(FTN.PowerTransformer cimPowerTransformer)
        //{
        //	ResourceDescription rd = null;
        //	if (cimPowerTransformer != null)
        //	{
        //		long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.POWERTR, importHelper.CheckOutIndexForDMSType(DMSType.POWERTR));
        //		rd = new ResourceDescription(gid);
        //		importHelper.DefineIDMapping(cimPowerTransformer.ID, gid);

        //		////populate ResourceDescription
        //		PowerTransformerConverter.PopulatePowerTransformerProperties(cimPowerTransformer, rd, importHelper, report);
        //	}
        //	return rd;
        //}

        //private void ImportTransformerWindings()
        //{
        //	SortedDictionary<string, object> cimTransformerWindings = concreteModel.GetAllObjectsOfType("FTN.TransformerWinding");
        //	if (cimTransformerWindings != null)
        //	{
        //		foreach (KeyValuePair<string, object> cimTransformerWindingPair in cimTransformerWindings)
        //		{
        //			FTN.TransformerWinding cimTransformerWinding = cimTransformerWindingPair.Value as FTN.TransformerWinding;

        //			ResourceDescription rd = CreateTransformerWindingResourceDescription(cimTransformerWinding);
        //			if (rd != null)
        //			{
        //				delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
        //				report.Report.Append("TransformerWinding ID = ").Append(cimTransformerWinding.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
        //			}
        //			else
        //			{
        //				report.Report.Append("TransformerWinding ID = ").Append(cimTransformerWinding.ID).AppendLine(" FAILED to be converted");
        //			}
        //		}
        //		report.Report.AppendLine();
        //	}
        //}

        //private ResourceDescription CreateTransformerWindingResourceDescription(FTN.TransformerWinding cimTransformerWinding)
        //{
        //	ResourceDescription rd = null;
        //	if (cimTransformerWinding != null)
        //	{
        //		long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.POWERTRWINDING, importHelper.CheckOutIndexForDMSType(DMSType.POWERTRWINDING));
        //		rd = new ResourceDescription(gid);
        //		importHelper.DefineIDMapping(cimTransformerWinding.ID, gid);

        //		////populate ResourceDescription
        //		PowerTransformerConverter.PopulateTransformerWindingProperties(cimTransformerWinding, rd, importHelper, report);
        //	}
        //	return rd;
        //}

        //private void ImportWindingTests()
        //{
        //	SortedDictionary<string, object> cimWindingTests = concreteModel.GetAllObjectsOfType("FTN.WindingTest");
        //	if (cimWindingTests != null)
        //	{
        //		foreach (KeyValuePair<string, object> cimWindingTestPair in cimWindingTests)
        //		{
        //			FTN.WindingTest cimWindingTest = cimWindingTestPair.Value as FTN.WindingTest;

        //			ResourceDescription rd = CreateWindingTestResourceDescription(cimWindingTest);
        //			if (rd != null)
        //			{
        //				delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
        //				report.Report.Append("WindingTest ID = ").Append(cimWindingTest.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
        //			}
        //			else
        //			{
        //				report.Report.Append("WindingTest ID = ").Append(cimWindingTest.ID).AppendLine(" FAILED to be converted");
        //			}
        //		}
        //		report.Report.AppendLine();
        //	}
        //}

        //private ResourceDescription CreateWindingTestResourceDescription(FTN.WindingTest cimWindingTest)
        //{
        //	ResourceDescription rd = null;
        //	if (cimWindingTest != null)
        //	{
        //		long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.WINDINGTEST, importHelper.CheckOutIndexForDMSType(DMSType.WINDINGTEST));
        //		rd = new ResourceDescription(gid);
        //		importHelper.DefineIDMapping(cimWindingTest.ID, gid);

        //		////populate ResourceDescription
        //		PowerTransformerConverter.PopulateWindingTestProperties(cimWindingTest, rd, importHelper, report);
        //	}
        //	return rd;
        //}


        #endregion Import
    }
}

