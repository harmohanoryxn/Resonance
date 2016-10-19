using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Cloudmaster.WCS.Galway.Core
{
	public class DetectionToRfidLocationMapper
	{
		public static string Translate(string detectionLocation)
		{
			switch (detectionLocation)
				{
					case "MRI":
						return "MRI";
					case "CT":
						return "CT";
					case "CT Scan":
						return "CT";
					case "Ultrasound":
						return "Radiology";
					case "X-Ray":
						return "Radiology";
					case "Nuclear Medicine":
						return "Radiology";
					case "Fluoroscopy":
						return "Radiology";
					case "Cardiology":
						return "Radiology";
					case "NEURO MOD":
						return "Radiology";
						break;
					case "Ward 1":
					case "Freyer":
						return "ACC1";
					case "Ward 2":
					case "Mother Teresa":
						return "ACC2";
					case "Ward 3":
					case "Our Lady Of Knock":
						return "ACC3";
					case "Ward 4":
					case "O Malley":
						return "ACCG";
					case "John Paul II":
					case "Florence Nightingale":
						return "ACCLG";
				case "Emergency Room":
						return "ER";
						break;
					default:
						return "nothingFloorplan";
//return "Ground_Floor";
//return "Attrium";
//return "Respirotory_Lab_Entrance";
//return "Angio_Entrance";
//return "Radiology_Entrance";
//return "Main_Entrance";
//return "AngioRecovery";
//return "ICU";
//return "Physiotherapy";
//
//return "Daycare";
//return "Theatre";
//return "TheatrePreOp";
//return "Radiotherapy";
//return "RL";
//return "Icon";
//return "Conference_Room";
				}
		}
	}
}















