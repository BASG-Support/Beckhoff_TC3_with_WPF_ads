using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Linq;

namespace ADS_Sample.UI_Config
{
    class SetupManager
    {
        public static void Load_DiagConfig()
        {
            XDocument DiagConfig = XDocument.Parse("<Diagnostic></Diagnostic>");
            XDocument PlcConfig = XDocument.Load(@"UI Config/PlcConfig.xml");
            try
            {
                foreach (XElement Diagnostic_Do in PlcConfig.Root.Descendants().Where(Var => Var.Attribute("Tag").Value.ToLower().Equals("diagnostic_do")))
                {
                    DiagConfig.Root.Add(new XElement("Do", new XAttribute("Name", Diagnostic_Do.Attribute("Name").Value), new XAttribute("Index", Diagnostic_Do.Attribute("Index").Value)));
                }
            }
            catch (Exception ex)
            {
                TwincatConnector.LogMessage(string.Format("{0} - {1}", "HMI Diagnostic page", "Unable to create DO :" + ex.Message));
            }
            try
            {
                foreach (XElement Diagnostic_Di in PlcConfig.Root.Descendants().Where(Var => Var.Attribute("Tag").Value.ToLower().Equals("diagnostic_di")))
                {
                    DiagConfig.Root.Add(new XElement("Di", new XAttribute("Name", Diagnostic_Di.Attribute("Name").Value), new XAttribute("Index", Diagnostic_Di.Attribute("Index").Value)));
                }
            }
            catch (Exception ex)
            {
                TwincatConnector.LogMessage(string.Format("{0} - {1}", "HMI Diagnostic page", "Unable to create DI :" + ex.Message));
            }
            try
            {
                foreach (XElement Diagnostic_Ao in PlcConfig.Root.Descendants().Where(Var => Var.Attribute("Tag").Value.ToLower().Equals("diagnostic_ao")))
                {
                    DiagConfig.Root.Add(new XElement("Ao", new XAttribute("Name", Diagnostic_Ao.Attribute("Name").Value), new XAttribute("Index", Diagnostic_Ao.Attribute("Index").Value)));
                }
            }
            catch (Exception ex)
            {
                TwincatConnector.LogMessage(string.Format("{0} - {1}", "HMI Diagnostic page", "Unable to create AO :" + ex.Message));
            }
            try
            {
                foreach (XElement Diagnostic_Ai in PlcConfig.Root.Descendants().Where(Var => Var.Attribute("Tag").Value.ToLower().Equals("diagnostic_ai")))
                {
                    DiagConfig.Root.Add(new XElement("Ai", new XAttribute("Name", Diagnostic_Ai.Attribute("Name").Value), new XAttribute("Index", Diagnostic_Ai.Attribute("Index").Value)));
                }
            }
            catch (Exception ex)
            {
                TwincatConnector.LogMessage(string.Format("{0} - {1}", "HMI Diagnostic page", "Unable to create AI :" + ex.Message));
            }
            DiagConfig.Save("UI Config/DiagPageConfig.xml");
        }
    }
}
