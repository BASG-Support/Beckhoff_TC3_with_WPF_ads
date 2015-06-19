using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Linq;

namespace ADS_Sample.UI_Config
{
    class ui_manager
    {
        public static void Load_DiagConfig()
        {
            XDocument DiagConfig = XDocument.Parse("<Diagnostic></Diagnostic>");
            XDocument PlcConfig = XDocument.Load(@"UI Config/tcConnectorConfig.xml");
            XDocument UiConfig = XDocument.Load(@"UI Config/UiConfig.xml");
            #region Diagnostics
            #region IO
            int _plcINDEX = 0;
            foreach (XElement Plc in PlcConfig.Root.Descendants("Plc"))
            {
                #region DO
                try
                {
                    foreach (XElement Diagnostic_Do in Plc.Descendants().Where(Var => Var.Attribute("Tag").Value.ToLower().Equals("diagnostic_do")))
                    {
                        DiagConfig.Root.Add(new XElement("Do", new XAttribute("Name", Diagnostic_Do.Attribute("Name").Value), new XAttribute("Index", Diagnostic_Do.Attribute("Index").Value), new XAttribute("Plc", _plcINDEX)));
                    }
                }
                catch (Exception ex)
                {
                    applog_manager.appLogMessage("SU", "Unable to create DO entry " + ex.Message);
                }
                #endregion
                #region DI
                try
                {
                    foreach (XElement Diagnostic_Di in Plc.Descendants().Where(Var => Var.Attribute("Tag").Value.ToLower().Equals("diagnostic_di")))
                    {
                        DiagConfig.Root.Add(new XElement("Di", new XAttribute("Name", Diagnostic_Di.Attribute("Name").Value), new XAttribute("Index", Diagnostic_Di.Attribute("Index").Value), new XAttribute("Plc", _plcINDEX)));
                    }
                }
                catch (Exception ex)
                {
                    applog_manager.appLogMessage("SU", "Unable to create DI entry " + ex.Message);
                }
                #endregion
                #region AO
                try
                {
                    foreach (XElement Diagnostic_Ao in PlcConfig.Descendants().Where(Var => Var.Attribute("Tag").Value.ToLower().Equals("diagnostic_ao")))
                    {
                        DiagConfig.Root.Add(new XElement("Ao", new XAttribute("Name", Diagnostic_Ao.Attribute("Name").Value), new XAttribute("Index", Diagnostic_Ao.Attribute("Index").Value), new XAttribute("Plc", _plcINDEX)));
                    }
                }
                catch (Exception ex)
                {
                    applog_manager.appLogMessage("SU", "Unable to create AO entry " + ex.Message);
                }
                #endregion
                #region AI
                try
                {
                    foreach (XElement Diagnostic_Ai in PlcConfig.Root.Element("Plc").Descendants().Where(Var => Var.Attribute("Tag").Value.ToLower().Equals("diagnostic_ai")))
                    {
                        DiagConfig.Root.Add(new XElement("Ai", new XAttribute("Name", Diagnostic_Ai.Attribute("Name").Value), new XAttribute("Index", Diagnostic_Ai.Attribute("Index").Value), new XAttribute("Plc", _plcINDEX)));
                    }
                }
                catch (Exception ex)
                {
                    applog_manager.appLogMessage("SU", "Unable to create AI entry " + ex.Message);
                }
                #endregion
                _plcINDEX++;
            }
            #endregion
            #region Polling interval
            try
            {
                DiagConfig.Root.Add(new XElement("Polling",new XElement("m",UiConfig.Root.Element("PollTime").Attribute("m").Value),new XElement("s",UiConfig.Root.Element("PollTime").Attribute("s").Value),new XElement("ms",UiConfig.Root.Element("PollTime").Attribute("ms").Value)));
            }
            catch (Exception)
            {
                DiagConfig.Root.Add(new XElement("Polling", new XElement("m", 0), new XElement("s", 0), new XElement("ms", 500)));
                applog_manager.appLogMessage("UC", "Unable to get diagnostics polling config, defaulting to 500ms");
            }
            DiagConfig.Save("UI Config/DiagPageConfig.xml");
            #endregion
            #endregion
        }
    }
}
