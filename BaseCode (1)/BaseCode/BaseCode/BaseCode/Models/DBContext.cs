using System;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Data;
using System.Linq.Expressions;
using System.Text;
using System.Net;
using System.Diagnostics.Eventing.Reader;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Runtime.CompilerServices;
using System.ComponentModel.Design;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Security.Policy;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using BaseCode.Models.Responses;
using BaseCode.Models.Requests;
using BaseCode.Models.Tables;

using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace BaseCode.Models
{
    public class DBContext
    {
        public string ConnectionString { get; set; }
        public DBContext(string connStr)
        {
            this.ConnectionString = connStr;            
        }
        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        public GenericInsertUpdateResponse InsertUpdateData(GenericInsertUpdateRequest r)
        {
            GenericInsertUpdateResponse resp = new GenericInsertUpdateResponse();
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    MySqlTransaction myTrans;
                    myTrans = conn.BeginTransaction();
                    MySqlCommand cmd = new MySqlCommand(r.query, conn);
                    cmd.ExecuteNonQuery();

                    resp.Id = r.isInsert ? int.Parse(cmd.LastInsertedId.ToString()) : -1;
                    myTrans.Commit();
                    conn.Close();
                    resp.isSuccess = true;
                    resp.Message = r.responseMessage;
                }
            }
            catch (Exception ex)
            {
                resp.isSuccess = false;
                resp.Message = r.errorMessage + ": " + ex.Message;
            }
            return resp;
        }
        public CreateUserResponse CreateUserUsingSqlScript(CreateUserRequest r)
        {
            
            GetDepListResponse resp_dep = new GetDepListResponse();
            resp_dep.DepList = new List<Dep>();
            CreateUserResponse resp_user = new CreateUserResponse();
            string query = "SELECT * FROM DEPARTMENT WHERE DEP_STATUS = 'A' AND DEP_ID = '" + r.dep_id + "' ";
            GenericGetDataResponse getData = new GenericGetDataResponse();
            getData.Data = new DataTable();
            getData = GetData(query);


            if (getData.Data.Rows.Count > 0)
            {
                GetUserListResponse resp = new GetUserListResponse();
                resp.UsersList = new List<User>();
                string query2 = "SELECT USER_NAME FROM USER WHERE USER_NAME = '" + r.UserName + "'";
                GenericGetDataResponse getData1 = new GenericGetDataResponse();
                getData1.Data = new DataTable();
                getData1 = GetData(query2);

                if (getData1.Data.Rows.Count > 0)
                {
                    GenericInsertUpdateRequest genReq = new GenericInsertUpdateRequest();

                    genReq.query = null;
                    genReq.isInsert = false;
                    genReq.responseMessage = " Username exist.";
                    genReq.errorMessage = " Username exist.";

                    GenericInsertUpdateResponse genResp = new GenericInsertUpdateResponse();

                    resp_user.Message = " Username exist.";
                    resp_user.isSuccess = genResp.isSuccess;
                    resp_user.UserId = genResp.Id;

                    return resp_user;
                }
                else
                {
                    string query1 = " INSERT INTO USER (FIRST_NAME, LAST_NAME, USER_NAME, PASSWORD, PHONE_NUMBER, DEPARTMENT_ID)";
                    query1 += "VALUES ('" + r.FirstName + "','" + r.LastName + "','" + r.UserName + "','" + r.Password + "' , '" + r.PhoneNumber + "','" + r.dep_id + "')";

                    GenericInsertUpdateRequest genReq = new GenericInsertUpdateRequest();
                    genReq.query = query1;
                    genReq.isInsert = true;
                    genReq.responseMessage = " Successfully created user.";
                    genReq.errorMessage = " Unable to create user";

                    GenericInsertUpdateResponse genResp = new GenericInsertUpdateResponse();
                    genResp = InsertUpdateData(genReq);

                    resp_user.Message = genResp.Message;
                    resp_user.isSuccess = genResp.isSuccess;
                    resp_user.UserId = genResp.Id;

                    return resp_user;
                    
                }


                    

            }
            else
            {
                GenericInsertUpdateRequest genReq = new GenericInsertUpdateRequest();

                genReq.query = null;
                genReq.isInsert = false;
                genReq.responseMessage = " Department ID does not exist.";
                genReq.errorMessage = " Department ID does not exist.";

                GenericInsertUpdateResponse genResp = new GenericInsertUpdateResponse();

                resp_user.Message = " Department ID does not exist.";
                resp_user.isSuccess = genResp.isSuccess;
                resp_user.UserId = genResp.Id;

                return resp_user;
            }

        }
        public GetUserListResponse LoginUser(GetUserListRequest r)
        {
            GetUserListResponse resp = new GetUserListResponse();
            resp.UsersList = new List<User>();
            User u;
            string query = "SELECT * FROM USER WHERE USER_NAME = '"+ r.UserName + "' AND PASSWORD = '"+ r.Password + "' ";
            GenericGetDataResponse getData = new GenericGetDataResponse();
            getData.Data = new DataTable();

            getData = GetData(query);


            if (getData.Data.Rows.Count > 0)
            {
                foreach (DataRow dr in getData.Data.Rows)
                {
                    u = new User();
                    u.UserId = int.Parse(dr["USER_ID"].ToString());
                    u.FirstName = dr["FIRST_NAME"].ToString();
                    u.LastName = dr["LAST_NAME"].ToString();
                    u.UserName = dr["USER_NAME"].ToString();
                    u.PhoneNumber = dr["PHONE_NUMBER"].ToString();

                    u.Status = dr["STATUS"].ToString() == "A" ? "ACTIVE" : "INACTIVE";
                    resp.UsersList.Add(u);

                }
                resp.isSuccess = true;
                resp.Message = "Login Successful!";
                return resp;
            }

            else
            {
                resp.isSuccess = true;
                resp.Message = " No info.";
                return resp;
            }
        }

        public CreateUserResponse UpdateUser(CreateUserRequest r)
        {
            CreateUserResponse resp = new CreateUserResponse();

            DateTime theDate = DateTime.Now;
            string crtdt = theDate.ToString("yyyy-MM-dd H:mm:ss");

            string query = "UPDATE USER SET ";
            query += !string.IsNullOrEmpty(r.FirstName) ? " FIRST_NAME = '"+ r.FirstName + "',": "";
            query += !string.IsNullOrEmpty(r.LastName) ? " LAST_NAME = '" + r.LastName  + "'," : "";
            query += !string.IsNullOrEmpty(r.UserName) ? " USER_NAME = '" + r.UserName  + "'," : "";
            query += !string.IsNullOrEmpty(r.Password) ? " PASSWORD = '"+  r.Password + "'," : "";
            query += !string.IsNullOrEmpty(r.PhoneNumber) ? "PHONE_NUMBER = '" + r.PhoneNumber + "'," : "";
            query += !string.IsNullOrEmpty(r.dep_id) ? "DEPARTMENT_ID = '" + r.dep_id + "'," : "";

            query += " UPDATE_DATE = '" + crtdt + "' WHERE USER_ID = " + r.UserId;


            GenericInsertUpdateRequest genReq = new GenericInsertUpdateRequest();

            genReq.query = query;
            genReq.isInsert = false;
            genReq.responseMessage = " Successfully updated user.";
            genReq.errorMessage = " Unable to update user";

            GenericInsertUpdateResponse genResp = new GenericInsertUpdateResponse();
            genResp = InsertUpdateData(genReq);

            resp.Message = genResp.Message;
            resp.isSuccess = genResp.isSuccess;
            resp.UserId = genResp.Id;

            return resp;
        }
        public GetUserListResponse GetUserList(GetUserListRequest r )
        {
            GetUserListResponse resp = new GetUserListResponse();
            resp.UsersList = new List<User>();
            User u;
            string query = "SELECT * FROM USER INNER JOIN DEPARTMENT WHERE USER.DEPARTMENT_ID = DEPARTMENT.DEP_ID";
            GenericGetDataResponse getData = new GenericGetDataResponse();
            getData.Data = new DataTable();

            getData = GetData(query);

            if(getData.Data.Rows.Count > 0)
            {
                foreach (DataRow dr in getData.Data.Rows)
                {
                    u = new User();
                    u.UserId = int.Parse(dr["USER_ID"].ToString());
                    u.FirstName = dr["FIRST_NAME"].ToString();
                    u.LastName = dr["LAST_NAME"].ToString();
                    u.UserName = dr["USER_NAME"].ToString();
                    u.PhoneNumber = dr["PHONE_NUMBER"].ToString();
                    if (dr["DEP_STATUS"].ToString() == "A")
                    {
                        u.DepName = dr["DEP_NAME"].ToString();
                    }
                    else
                    {
                        u.DepName = "";
                    }
                    
                    u.Status = dr["STATUS"].ToString() == "A" ? "ACTIVE" : "INACTIVE";
                    resp.UsersList.Add(u);

                }
                resp.isSuccess = true;
                resp.Message = "List Of Users";
                return resp;
            }

            else
            {
                resp.isSuccess = true;
                resp.Message = " No users found.";
                return resp;
            }



        }

        public CreateDepResponse CreateDepUsingSqlScript(CreateDepRequest r)
        {
            CreateDepResponse resp = new CreateDepResponse();
            string query = " INSERT INTO DEPARTMENT (DEP_NAME, DEP_DESCRIPTION)";
            query += "VALUES ('" + r.DepName + "','" + r.DepDescription + "')";

            GenericInsertUpdateRequest genReq = new GenericInsertUpdateRequest();

            genReq.query = query;
            genReq.isInsert = true;
            genReq.responseMessage = " Successfully created department.";
            genReq.errorMessage = " Unable to create department";

            GenericInsertUpdateResponse genResp = new GenericInsertUpdateResponse();
            genResp = InsertUpdateData(genReq);

            resp.Message = genResp.Message;
            resp.isSuccess = genResp.isSuccess;
            resp.DepId = genResp.Id;

            return resp;
        }

        public CreateDepResponse UpdateDep(CreateDepRequest r)
        {
            CreateDepResponse resp = new CreateDepResponse();

            DateTime theDate = DateTime.Now;
            string crtdt = theDate.ToString("yyyy-MM-dd H:mm:ss");

            string query = "UPDATE DEPARTMENT SET ";
            query += !string.IsNullOrEmpty(r.DepName) ? " DEP_NAME = '" + r.DepName + "'," : "";
            query += !string.IsNullOrEmpty(r.DepDescription) ? " DEP_DESCRIPTION = '" + r.DepDescription + "'," : "";
            query += !string.IsNullOrEmpty(r.DepStatus) ? " DEP_STATUS = '" + r.DepStatus + "'," : "";

            query += " DEP_UPDATED_DATE = '" + crtdt + "' WHERE DEP_ID = " + r.DepId;


            GenericInsertUpdateRequest genReq = new GenericInsertUpdateRequest();

            genReq.query = query;
            genReq.isInsert = false;
            genReq.responseMessage = " Successfully updated Department.";
            genReq.errorMessage = " Unable to update Department";

            GenericInsertUpdateResponse genResp = new GenericInsertUpdateResponse();
            genResp = InsertUpdateData(genReq);

            resp.Message = genResp.Message;
            resp.isSuccess = genResp.isSuccess;
            resp.DepId = genResp.Id;

            return resp;
        }

        public CreateDepResponse DeleteDep(CreateDepRequest r)
        {
            CreateDepResponse resp = new CreateDepResponse();

            DateTime theDate = DateTime.Now;
            string crtdt = theDate.ToString("yyyy-MM-dd H:mm:ss");

            string query = "UPDATE DEPARTMENT SET ";
            query += !string.IsNullOrEmpty(r.DepName) ? " DEP_NAME = '" + r.DepName + "'," : "";

            query += " DEP_UPDATED_DATE = '" + crtdt + "' WHERE DEP_ID = " + r.DepId;


            GenericInsertUpdateRequest genReq = new GenericInsertUpdateRequest();

            genReq.query = query;
            genReq.isInsert = false;
            genReq.responseMessage = " Successfully updated Department.";
            genReq.errorMessage = " Unable to update Department";

            GenericInsertUpdateResponse genResp = new GenericInsertUpdateResponse();
            genResp = InsertUpdateData(genReq);

            resp.Message = genResp.Message;
            resp.isSuccess = genResp.isSuccess;
            resp.DepId = genResp.Id;

            return resp;
        }

        public GetDepListResponse GetDepList(GetDepListRequest r)
        {
            GetDepListResponse resp = new GetDepListResponse();
            resp.DepList = new List<Dep>();
            Dep u;
            string query = "SELECT * FROM DEPARTMENT WHERE DEP_STATUS = 'A'";
            GenericGetDataResponse getData = new GenericGetDataResponse();
            getData.Data = new DataTable();

            getData = GetData(query);

            if (getData.Data.Rows.Count > 0)
            {
                foreach (DataRow dr in getData.Data.Rows)
                {
                    u = new Dep();
                    u.DepId = int.Parse(dr["DEP_ID"].ToString());
                    u.DepName = dr["DEP_NAME"].ToString();
                    u.DepDescription = dr["DEP_DESCRIPTION"].ToString();
                    u.DepStatus = dr["DEP_STATUS"].ToString() == "A" ? "ACTIVE" : "DEACTIVE" ;
                    resp.DepList.Add(u);
                }
                resp.isSuccess = true;
                resp.Message = "List Of Departments";
                return resp;
            }
            else
            {
                resp.isSuccess = true;
                resp.Message = " No departments found.";
                return resp;
            }


        }
        public GenericGetDataResponse GetData(string query)
        {
            GenericGetDataResponse resp = new GenericGetDataResponse();
            DataTable dt;
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    dt = new DataTable();
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        dt.Load(reader);
                        reader.Close();
                    }
                    conn.Close();
                }
                resp.isSuccess = true;
                resp.Message = "Successfully get data";
                resp.Data = dt;

            }
            catch (Exception ex)
            {
                resp.isSuccess = false;
                resp.Message = ex.Message;
            }
            return resp;
        }
    }
}
