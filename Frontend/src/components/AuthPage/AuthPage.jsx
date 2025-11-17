import React, { useEffect, useState } from "react";
import GoogleButton from "./GoogleButton";
import "./auth.css"; 


const API_BASE =
  (typeof import.meta !== "undefined" &&
    import.meta.env &&
    import.meta.env.VITE_API_URL) ||
  process.env.REACT_APP_API_URL ||
  "http://localhost:5000"


export default function AuthPage({ onLoginSuccess }) {
  const [mode, setMode] = useState("login"); 
  const [status, setStatus] = useState("");

  
  const [loginEmail, setLoginEmail] = useState("");
  const [loginPassword, setLoginPassword] = useState("");

  
  const [regNombre, setRegNombre] = useState("");
  const [regEmail, setRegEmail] = useState("");
  const [regPassword, setRegPassword] = useState("");

  
  const [isWide, setIsWide] = useState(window.innerWidth > 850);

  useEffect(() => {
    const handleResize = () => setIsWide(window.innerWidth > 850);
    window.addEventListener("resize", handleResize);
    return () => window.removeEventListener("resize", handleResize);
  }, []);

  const handleLoginSubmit = async (e) => {
    e.preventDefault();
    setStatus("");

    try {
      const resp = await fetch(`${API_BASE}/auth/login`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        credentials: "include", 
        body: JSON.stringify({
          email: loginEmail,
          password: loginPassword,
        }),
      });

      if (!resp.ok) {
        const msg = await resp.json().catch(() => ({}));
        setStatus(msg.message || "Error al iniciar sesión ❌");
        return;
      }

      const data = await resp.json();
      localStorage.setItem("token", data.token);
      localStorage.setItem("user", JSON.stringify(data.user));

      setStatus(`Bienvenido ${data.user.nombre || data.user.email} ✅`);
      onLoginSuccess?.(data.user);
    } catch (err) {
      console.error(err);
      setStatus("Error de conexión con el servidor ❌");
    }
  };

  const handleRegisterSubmit = async (e) => {
    e.preventDefault();
    setStatus("");

    try {
      const resp = await fetch(`${API_BASE}/auth/register`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify({
          nombre: regNombre,
          email: regEmail,
          password: regPassword,
        }),
      });

      if (!resp.ok) {
        const msg = await resp.json().catch(() => ({}));
        setStatus(msg.message || "Error al registrarse ❌");
        return;
      }

      const data = await resp.json();
      localStorage.setItem("token", data.token);
      localStorage.setItem("user", JSON.stringify(data.user));

      setStatus(`Cuenta creada, bienvenido ${data.user.nombre || data.user.email} ✅`);
      onLoginSuccess?.(data.user);
      
      setMode("login");
    } catch (err) {
      console.error(err);
      setStatus("Error de conexión con el servidor ❌");
    }
  };

  
  const containerStyle = {
    left:
      isWide
        ? mode === "login"
          ? "10px"
          : "410px"
        : "0px",
  };

  const showLogin = mode === "login";
  const showRegister = mode === "register";

  return (
    <main>
      <div className="container__all">
        <div className="back__box">
          <div className="back__box-login">
            <h3>¿Ya tienes una cuenta?</h3>
            <p>Inicia sesión para acceder</p>
            <button id="btn__log" onClick={() => setMode("login")}>
              Iniciar sesión
            </button>
          </div>
          <div className="back__box-register">
            <h3>¿Aún no tienes una cuenta?</h3>
            <p>Regístrate para iniciar sesión</p>
            <button id="btn__register" onClick={() => setMode("register")}>
              Registrarse
            </button>
          </div>
        </div>

        <div
          className="container__login-register"
          style={containerStyle}
        >
          {/* LOGIN */}
          <form
            className="form__login"
            style={{ display: showLogin ? "block" : "none" }}
            onSubmit={handleLoginSubmit}
          >
            <h2>Iniciar sesión</h2>
            <input
              type="email"
              placeholder="Correo electrónico"
              value={loginEmail}
              onChange={(e) => setLoginEmail(e.target.value)}
              required
            />
            <input
              type="password"
              placeholder="Contraseña"
              value={loginPassword}
              onChange={(e) => setLoginPassword(e.target.value)}
              required
            />
            <button type="submit">Entrar</button>

            <div style={{ marginTop: "20px", textAlign: "center" }}>
              <p style={{ marginBottom: "10px" }}>O continúa con:</p>
              <GoogleButton
                onResult={(ok, msg) =>
                  setStatus(ok ? `Google OK ✅: ${msg}` : `Google ERROR ❌: ${msg}`)
                }
                onLoginUser={(user) => {
                  if (user) {
                    localStorage.setItem("user", JSON.stringify(user));
                    setStatus(`Bienvenido ${user.nombre || user.email} ✅`);
                    onLoginSuccess?.(user);
                  } else {
                    setStatus("Error: no se recibió usuario de Google.");
                  }
                }}
              />
            </div>
          </form>

          {/* REGISTER */}
          <form
            className="form__register"
            style={{ display: showRegister ? "block" : "none" }}
            onSubmit={handleRegisterSubmit}
          >
            <h2>Registrarse</h2>
            <input
              type="text"
              placeholder="Nombre completo"
              value={regNombre}
              onChange={(e) => setRegNombre(e.target.value)}
              required
            />
            <input
              type="email"
              placeholder="Correo electrónico"
              value={regEmail}
              onChange={(e) => setRegEmail(e.target.value)}
              required
            />
            {/* si querés username aparte, podrías agregar otro input, pero tu backend no lo guarda aún */}
            <input
              type="password"
              placeholder="Contraseña"
              value={regPassword}
              onChange={(e) => setRegPassword(e.target.value)}
              required
            />
            <button type="submit">Registrarse</button>
          </form>
        </div>
      </div>

      {status && (
        <p
          style={{
            marginTop: "20px",
            textAlign: "center",
            color: "#fff",
            textShadow: "0 0 4px #000",
          }}
        >
          {status}
        </p>
      )}
    </main>
  );
}
