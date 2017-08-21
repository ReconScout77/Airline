using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;

namespace Airline.Models
{
  public class City
  {
    private int _id;
    private string _cityName;

    public City(string location, int id=0)
    {
      _id = id;
      _cityName = location;
    }

    ////GETTERS
    public int GetId()
    {
      return _id;
    }

    public string GetCityName()
    {
      return _cityName;
    }
    ////GETTERS END

    public override bool Equals(System.Object otherCity)
    {
      if (!(otherCity is City))
      {
        return false;
      }
      else
      {
        City newCity = (City) otherCity;
        bool idEquality = this.GetId() == newCity.GetId();
        bool cityEquality = this.GetCityName() == newCity.GetCityName();
        return (idEquality && cityEquality);
      }
    }

    public override int GetHashCode()
    {
      return this.GetCityName().GetHashCode();
    }

    public void Save()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO cities (cityName) VALUES (@place);";

      MySqlParameter cityName = new MySqlParameter();
      cityName.ParameterName = "@place";
      cityName.Value = this._cityName;
      cmd.Parameters.Add(cityName);

      cmd.ExecuteNonQuery();
      _id = (int) cmd.LastInsertedId;
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public static List<City> GetAll()
    {
      List<City> allCities = new List<City> {};
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM cities;";

      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
        int cityId = rdr.GetInt32(0);
        string cityName = rdr.GetString(1);
        City newCity = new City(cityName, cityId);
        allCities.Add(newCity);
      }

      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return allCities;
    }

    public static City Find(int id)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM cities WHERE id = @searchId;";

      MySqlParameter searchId = new MySqlParameter();
      searchId.ParameterName = "@searchId";
      searchId.Value = id;
      cmd.Parameters.Add(searchId);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      int cityId = 0;
      string cityName = "";

      while(rdr.Read())
      {
        cityId = rdr.GetInt32(0);
        cityName = rdr.GetString(1);
      }
      City foundCity = new City(cityName, cityId);

      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return foundCity;
    }

    public void Delete()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM cities WHERE id = @searchId;";

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

    public void DeleteAll()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM cities";

      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public void AddFlight(Flight newFlight)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO cities_flights (city_id, flight_id) VALUES (@cityId, @flightId);";

      MySqlParameter city_id = new MySqlParameter();
      city_id.ParameterName = "@cityId";
      city_id.Value = _id;
      cmd.Parameters.Add(city_id);

      MySqlParameter flight_id = new MySqlParameter();
      flight_id.ParameterName = "@flightId";
      flight_id.Value = newFlight.GetId();
      cmd.Parameters.Add(flight_id);

      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public List<Flight> GetFlights()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT flight_id FROM cities_flights WHERE city_id = @cityId;";

      MySqlParameter cityIdParameter = new MySqlParameter();
      cityIdParameter.ParameterName = "@cityId";
      cityIdParameter.Value = _id;
      cmd.Parameters.Add(cityIdParameter);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;

      //Find Flight IDs linked to city ID
      List<int> flightIds = new List<int> {};
      while(rdr.Read())
      {
        int flightId = rdr.GetInt32(0);
        flightIds.Add(flightId);
      }
      rdr.Dispose();

      //Retrieve list of Flight info from Flight ID list
      List<Flight> flights = new List<Flight> {};
      foreach (int flightId in flightIds)
      {
        var flightQuery = conn.CreateCommand() as MySqlCommand;
        flightQuery.CommandText = @"SELECT * FROM flights WHERE id = @flightId;";

        MySqlParameter flightIdParameter = new MySqlParameter();
        flightIdParameter.ParameterName = "@flightId";
        flightIdParameter.Value = flightId;
        flightQuery.Parameters.Add(flightIdParameter);

        var flightQueryRdr = flightQuery.ExecuteReader() as MySqlDataReader;
        while(flightQueryRdr.Read())
        {
          int thisFlightId = flightQueryRdr.GetInt32(0);
          string departTime = flightQueryRdr.GetString(1);
          string departCity = flightQueryRdr.GetString(2);
          string destination = flightQueryRdr.GetString(3);
          string status = flightQueryRdr.GetString(4);
          Flight foundFlight = new Flight(departTime, departCity, destination, status, thisFlightId);
          flights.Add(foundFlight);
        }
        flightQueryRdr.Dispose();
      }

      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return flights;
    }
  }
}
