﻿//#define USE_MYSQL

/*
 * SlimNet - Networking Middleware For Games
 * Copyright (C) 2011-2012 Fredrik Holmström
 * 
 * This notice may not be removed or altered.
 * 
 * This software is provided 'as-is', without any expressed or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software. 
 * 
 * Attribution
 * The origin of this software must not be misrepresented; you must not
 * claim that you wrote the original software. For any works using this 
 * software, reasonable acknowledgment is required.
 * 
 * Noncommercial
 * You may not use this software for commercial purposes.
 * 
 * Distribution
 * You are not allowed to distribute or make publicly available the software 
 * itself or its source code in original or modified form.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if USE_MYSQL
using MySql.Data.MySqlClient;
#endif
using SlimNet;

public class Database
{
#if USE_MYSQL
    MySqlConnection db;
#endif

    public readonly string ConnectionString;

    public Database(string host, string database, string user, string password)
    {
#if USE_MYSQL
        ConnectionString = "Server=" + host + ";Database=" + database + ";User ID=" + user + ";Password=" + password + ";Pooling=false";
        db = new MySqlConnection(ConnectionString);
        db.Open();
#endif
    }

    public void LogoutAllUsers()
    {
#if USE_MYSQL
        MySqlCommand cmd = db.CreateCommand();
        cmd.CommandText = "UPDATE users SET loggedin = 0";
        cmd.ExecuteNonQuery();
#endif
    }

    public string CreateAccount(string email, string password)
    {
#if USE_MYSQL
        if (email == "")
        {
            return "No email specified";
        }

        if (password == "")
        {
            return "No password specified";
        }

        try
        {
            MySqlCommand cmd = db.CreateCommand();

            cmd.CommandText = "INSERT INTO users (`email`, `password`) VALUES(?email, ?password)";
            cmd.Parameters.AddWithValue("?email", email);
            cmd.Parameters.AddWithValue("?password", password);
            cmd.ExecuteNonQuery();

            return "";
        }
        catch (Exception exn)
        {
            return exn.Message;
        }
#else
        return "";
#endif
    }

    public string Login(string email, string password, Player player)
    {
#if USE_MYSQL
        if (email == "")
        {
            return "No email specified";
        }

        if (password == "")
        {
            return "No password specified";
        }

        try
        {
            MySqlCommand cmd = db.CreateCommand();

            cmd.CommandText = "SELECT id, loggedin FROM users WHERE `email` = ?email AND `password` = ?password";
            cmd.Parameters.AddWithValue("?email", email);
            cmd.Parameters.AddWithValue("?password", password);

            MySqlDataReader reader = cmd.ExecuteReader();

            {
                if (reader.Read())
                {
                    int id = (int)reader[0];
                    int loggedin = (int)reader[1];

                    reader.Close();

                    if (loggedin == 0)
                    {
                        cmd = db.CreateCommand();
                        cmd.CommandText = "UPDATE users SET loggedin = 1 WHERE id = ?id";
                        cmd.Parameters.AddWithValue("?id", id);
                        cmd.ExecuteNonQuery();

                        (player.Tag as PlayerData).AccountId = id;
                    }
                    else
                    {
                        return "Account already logged in";
                    }
                }
                else
                {
                    reader.Close();
                }
            }

            return "";
        }
        catch (Exception exn)
        {
            return exn.Message;
        }
#else
        return "";
#endif
    }
}
