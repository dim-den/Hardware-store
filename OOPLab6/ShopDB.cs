using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OOPLab6
{
    public class ShopDB : IDisposable
    {
        string connectionString;
        SqlConnection connection = null;

        public ShopDB()
        {
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            connection = new SqlConnection(connectionString);
            connection.Open();
        }

        ~ShopDB()
        {
            if (connection != null)
                connection.Close();
        }

        public void Dispose()
        {
            if (connection != null)
                connection.Close();
        }

        public bool InsertDevice(Device d)
        {
            string sql = $"INSERT INTO DEVICE(NAME, IMAGEPATH, DESCRIPTION, PRODUCER, COUNTRY, QUANTITY, PURCHASED, PRICE) VALUES " +
                         $"(\'{d.Name}\', \'{d.ImagePath}\', \'{d.Description}\', \'{d.Producer}\', \'{d.Country}\', " +
                         $"{d.Quantity}, {d.Purhased}, {d.Price})";

            try
            {
                SqlCommand command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public bool DeleteDevice(Device d)
        {
            string sql = $"DELETE FROM DEVICE WHERE ID = {d.ID}";

            try
            {
                SqlCommand command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public List<Device> GetDevices()
        {
            string sql = "SELECT * FROM DEVICE";
            DataTable deviceTable = new DataTable();
            SqlDataAdapter adapter;

            List<Device> devices = new List<Device>();
            try
            {
                SqlCommand command = new SqlCommand(sql, connection);
                adapter = new SqlDataAdapter(command);
                adapter.Fill(deviceTable);

                for (int i = 0; i < deviceTable.Rows.Count; i++)
                {
                    Device device = new Device(
                         deviceTable.Rows[i].Field<int>("ID"),
                         deviceTable.Rows[i].Field<string>("NAME"),
                         deviceTable.Rows[i].Field<string>("IMAGEPATH"),
                         deviceTable.Rows[i].Field<string>("DESCRIPTION"),
                         deviceTable.Rows[i].Field<string>("PRODUCER"),
                         deviceTable.Rows[i].Field<string>("COUNTRY"),
                         deviceTable.Rows[i].Field<int>("QUANTITY"),
                         deviceTable.Rows[i].Field<int>("PURCHASED"),
                         deviceTable.Rows[i].Field<int>("PRICE")
                    );
                    devices.Add(device);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return devices;
        }

        public bool UpdateDevice(int id, Device device)
        {
            string sql =
                $"UPDATE DEVICE SET NAME = \'{device.Name}\', DESCRIPTION = \'{device.Description}\', PRICE = {device.Price}, " +
                $"QUANTITY = {device.Quantity}, PURCHASED = {device.Purhased}, PRODUCER = \'{device.Producer}\', COUNTRY = \'{device.Country}\' " +
                $"WHERE ID = {id}";
            try
            {
                SqlCommand command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }

        }
    }
}
