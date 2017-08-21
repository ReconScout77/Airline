using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;

namespace Airline.Models
{
  public class Flight
  {
    private int _id;
    private string _departureTime;
    private string _departureCity;
    private string _destination;
    private string _status;

    public Flight (string startTime, string startCity, string destination, string status, int id=0)
    {
      _id = id;
      _departureTime = startTime;
      _departureCity = startCity;
      _destination = destination;
      _status = status;
    }

    ////GETTERS
    public int GetId()
    {
      return _id;
    }

    public string GetDepartureTime()
    {
      return _departureTime;
    }

    public string GetDepartureCity()
    {
      return _departureCity;
    }

    public string GetDestination()
    {
      return _destination;
    }

    public string GetStatus()
    {
      return _status;
    }
    ////GETTERS END

    public override bool Equals(System.Object otherFlight)
    {
      if (!(otherFlight is Flight))
      {
        return false;
      }
      else
      {
        Flight newFlight = (Flight) otherFlight;
        bool idEquality = this.GetId() == newFlight.GetId();
        bool departTimeEquality = this.GetDepartureTime() == newFlight.GetDepartureTime();
        bool departCityEquality = this.GetDepartureCity() == newFlight.GetDepartureCity();
        bool destinationEquality = this.GetDestination() == newFlight.GetDestination();
        bool statusEquality = this.GetStatus() == newFlight.GetStatus();
        return (idEquality && departTimeEquality && departCityEquality && destinationEquality && statusEquality);
      }
    }

    public override int GetHashCode()
    {
      return this.GetDepartureCity().GetHashCode();
    }

    public void Save()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO flights (departTime, departCity, destination, status) VALUES (@departure_time, @departure_city, @destination, @status);";

      MySqlParameter departTime = new MySqlParameter();
      departTime.ParameterName = "@departure_time";
      departTime.Value = this._departureTime;
      cmd.Parameters.Add(departTime);

      MySqlParameter departCity = new MySqlParameter();
      departCity.ParameterName = "@departure_city";
      departCity.Value = this._departureCity;
      cmd.Parameters.Add(departCity);

      MySqlParameter destination = new MySqlParameter();
      destination.ParameterName = "@destination";
      destination.Value = this._destination;
      cmd.Parameters.Add(destination);

      MySqlParameter status = new MySqlParameter();
      status.ParameterName = "@status";
      status.Value = this._status;
      cmd.Parameters.Add(status);

      cmd.ExecuteNonQuery();
      _id = (int) cmd.LastInsertedId;
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public static List<Flight> GetAll()
    {
      List<Flight> allFlights = new List<Flight> {};
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM flights;";

      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      while (rdr.Read())
      {
        int flightId = rdr.GetInt32(0);
        string departureTime = rdr.GetString(1);
        string departureCity = rdr.GetString(2);
        string destination = rdr.GetString(3);
        string status = rdr.GetString(4);
        Flight newFlight = new Flight(departureTime, departureCity, destination, status, flightId);
        allFlights.Add(newFlight);
      }

      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return allFlights;
    }

    public static Flight Find(int id)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM flights WHERE id = @searchId;";

      MySqlParameter searchId = new MySqlParameter();
      searchId.ParameterName = "@searchId";
      searchId.Value = id;
      cmd.Parameters.Add(searchId);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      int flightId = 0;
      string departTime = "";
      string departCity = "";
      string destination = "";
      string status = "";

      while(rdr.Read())
      {
        flightId = rdr.GetInt32(0);
        departTime = rdr.GetString(1);
        departCity = rdr.GetString(2);
        destination = rdr.GetString(3);
        status = rdr.GetString(4);
      }
      Flight foundFlight = new Flight(departTime, departCity, destination, status, flightId);

      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return foundFlight;
    }

    public void Delete()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM flights WHERE id = @searchId;";

      MySqlParameter searchId = new MySqlParameter();
      searchId.ParameterName = "@searchId";
      searchId.Value = _id;
      cmd.Parameters.Add(searchId);

      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public static void DeleteAll()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM flights;";

      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public void AddCity(City newCity)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO cities_flights (city_id, flight_id) VALUES (@cityId, @flightId);";

      MySqlParameter city_id = new MySqlParameter();
      city_id.ParameterName = "@cityId";
      city_id.Value = newCity.GetId();
      cmd.Parameters.Add(city_id);

      MySqlParameter flight_id = new MySqlParameter();
      flight_id.ParameterName = "@flightId";
      flight_id.Value = _id;
      cmd.Parameters.Add(flight_id);

      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public List<City> GetCities()
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();
            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"SELECT city_id FROM cities_flights WHERE flight_id = @flightId;";

            MySqlParameter flightIdParameter = new MySqlParameter();
            flightIdParameter.ParameterName = "@flightId";
            flightIdParameter.Value = _id;
            cmd.Parameters.Add(flightIdParameter);

            var rdr = cmd.ExecuteReader() as MySqlDataReader;

            List<int> cityIds = new List<int> {};
            while(rdr.Read())
            {
                int cityId = rdr.GetInt32(0);
                cityIds.Add(cityId);
            }
            rdr.Dispose();

            List<City> cities = new List<City> {};
            foreach (int cityId in cityIds)
            {
                var cityQuery = conn.CreateCommand() as MySqlCommand;
                cityQuery.CommandText = @"SELECT * FROM cities WHERE id = @cityId;";

                MySqlParameter cityIdParameter = new MySqlParameter();
                cityIdParameter.ParameterName = "@cityId";
                cityIdParameter.Value = cityId;
                cityQuery.Parameters.Add(cityIdParameter);

                var cityQueryRdr = cityQuery.ExecuteReader() as MySqlDataReader;
                while(cityQueryRdr.Read())
                {
                    int thisCityId = cityQueryRdr.GetInt32(0);
                    string cityName = cityQueryRdr.GetString(1);
                    City foundCity = new City(cityName, thisCityId);
                    cities.Add(foundCity);
                }
                cityQueryRdr.Dispose();
            }
            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }
            return cities;
        }
  }
}
