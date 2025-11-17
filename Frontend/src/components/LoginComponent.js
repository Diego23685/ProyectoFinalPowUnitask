import React, { useEffect, useState } from "react";
import "../App.css";
import AuthPage from "../components/AuthPage/AuthPage.jsx";   // 👈 CAMBIO
import HomeScreen from "../components/HomeScreen";

export default function App() {
  const [user, setUser] = useState(null);

  useEffect(() => {
    const saved = localStorage.getItem("user");
    if (saved) {
      try { setUser(JSON.parse(saved)); }
      catch { localStorage.removeItem("user"); }
    }
  }, []);

  const handleLogout = () => {
    localStorage.removeItem("user");
    localStorage.removeItem("token");     // 👈 el backend guarda "token", no "auth_token"
    setUser(null);
  };

  return (
    <div className="App">
      {!user ? (
        <AuthPage onLoginSuccess={setUser} />  
      ) : (
        <HomeScreen user={user} onLogout={handleLogout} />
      )}
    </div>
  );
}
