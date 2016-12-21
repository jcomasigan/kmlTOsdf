using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;


namespace kmlTOsdf
{
    class kmlToSDF
    {
        public static void Reader()
        {
            DataTable PolyStyles = new DataTable();
            PolyStyles = FormatTable(PolyStyles);


            string xmlLocation = "H:\\investViewer_Data\\new geo\\Geological_units.kml";
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlLocation);
            XmlNodeList geolList = doc.GetElementsByTagName("Placemark");
            XmlNodeList styleList = doc.GetElementsByTagName("Style");
            foreach (XmlElement innerNode in geolList)
            {
                string _polygonPoints = "";
                string _geology = "";
                string _polyStyle = "";


                XmlNodeList coordNodes = innerNode.GetElementsByTagName("coordinates");
                XmlNodeList geology = innerNode.GetElementsByTagName("name");
                XmlNodeList polyStyle = innerNode.GetElementsByTagName("styleUrl");
                foreach (XmlElement coordinates in coordNodes)
                {
                    _polygonPoints = coordinates.InnerText;
                }
                foreach (XmlElement geo in geology)
                {
                    _geology = geo.InnerText;
                }
                foreach (XmlElement pStyle in polyStyle)
                {
                    _polyStyle = pStyle.InnerText;
                    foreach (XmlElement style in styleList)
                    {
                        XmlAttribute id = style.Attributes[0];
                        string val = id.Value;
                        if (val == pStyle.InnerText)
                        {
                            XmlNodeList colorList = style.GetElementsByTagName("color");
                            _polyStyle = colorList[1].InnerText;
                        }
                    }
                }

                //GeologyTable.Rows.Add(_geology, _polyStyle, _polygonPoints);
                AddToDb(_geology, _polyStyle, _polygonPoints);
            }
        }

        private static void AddToDb(string geo, string style, string points)
        {
            try
            {
                string ConnectionString = @"Data Source=" + @"H:\investViewer_Data\new geo\Geol.sdf";
                SqlCeConnection db = new SqlCeConnection(ConnectionString);
                if (points == "")
                {
                }
                db.Open();
                //using (SqlCeCommand cmd = new SqlCeCommand("INSERT INTO AucklandGeology (Geology,PolyStyle,PolyPoints) VALUES ('" + geo + "','" + style + "','" + points + "')", db))
                using (SqlCeCommand cmd = new SqlCeCommand("INSERT INTO AucklandGeology (Geology,PolyStyle,PolyPoints) VALUES (@Geo,@Style,@Points )", db))
                {
                    cmd.Parameters.AddWithValue("@Geo", geo);
                    cmd.Parameters.AddWithValue("@Style", style);
                    cmd.Parameters.AddWithValue("@Points", points);
                    cmd.ExecuteNonQuery();
                }
                db.Close();

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        private static DataTable FormatTable(DataTable dt)
        {
            dt = new DataTable();
            dt.Columns.Add("Geology", typeof(string));
            dt.Columns.Add("PolyStyle", typeof(string));
            dt.Columns.Add("Poly", typeof(string));
            return dt;
        }

    }
}
