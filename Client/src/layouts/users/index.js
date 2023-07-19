/**
=========================================================
* Material Dashboard 2 React - v2.1.0
=========================================================

* Product Page: https://www.creative-tim.com/product/material-dashboard-react
* Copyright 2022 Creative Tim (https://www.creative-tim.com)

Coded by www.creative-tim.com

 =========================================================

* The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
*/

import { Container } from "@mui/material";
import { useState, useContext, useEffect } from "react";
import { MainStateContext } from "App";
import { EvalueContext } from "context/evalueVariables";

// Material Dashboard 2 React contexts
import { useMaterialUIController, setDirection } from "context";
import UsersTable from "./UsersTable";


function Users() {
  const [users, setUsers] = useState([]);
  const [, dispatch] = useMaterialUIController();
  const { mainState } = useContext(MainStateContext);
  const { API } = useContext(EvalueContext);

  // bring all the users using GET api
  useEffect(() => {
    const abortController = new AbortController()
    if (mainState.is_Admin) {
      fetch(
        API.apiUserUrl,
        {
          method: "GET",
          headers: new Headers({
            "Content-Type": "application/json; charset=UTF-8",
            Accept: "application/json; charset=UTF-8",
          }),
          body: undefined,
          signal: abortController.signal
        })
        .then(async response => {
          const data = await response.json();
          console.log(response);

          if (!response.ok) {
            // get error message from body or default to response statusText
            const error = (data && data.message) || response.statusText;
            return Promise.reject(error);
          }

          return data;
        })
        .then(
          (result) => {
            console.log("success");
            setUsers(result);
          },
          (error) => {
            if (error.name === "AbortError") return
            console.log("err get=", error);
            throw error
          }
        );
    }
    return () => {
      abortController.abort()
      // stop the query by aborting on the AbortController on unmount
    }
  }, []);

  // Changing the direction to rtl
  useEffect(() => {
    setDirection(dispatch, "rtl");
    return () => setDirection(dispatch, "ltr");
  }, []);

  return (
    <Container maxWidth="xl" sx={{ pt: 3, pb: 3 }}>
      <UsersTable users={users} setUsers={setUsers} />
    </Container>
  );
}

export default Users;
