import React, { useState } from "react";
import "../App.css";
import GoogleButton from "./GoogleButton";

export default function LoginComponent({ onLoginSuccess }) {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [remember, setRemember] = useState(true);
  const [status, setStatus] = useState("");

  const onSubmit = (e) => {
    e.preventDefault();
    if (!email || !password) {
      setStatus("Debes ingresar correo y contraseña ❌");
      return;
    }
    // Simulación login local
    const fakeUser = { id: "local", nombre: "Usuario Local", email };
    localStorage.setItem("user", JSON.stringify(fakeUser));
    onLoginSuccess?.(fakeUser);
    setStatus("Inicio de sesión exitoso ✅");
  };

  return (
    <div className="page">
      <form className="card" onSubmit={onSubmit}>
        <h1>Iniciar sesión</h1>

        <label htmlFor="email">Correo</label>
        <input
          id="email"
          type="email"
          placeholder="tucorreo@ejemplo.com"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
        />

        <label htmlFor="password">Contraseña</label>
        <input
          id="password"
          type="password"
          placeholder="••••••••"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
        />

        <div className="row">
          <label className="remember">
            <input
              type="checkbox"
              checked={remember}
              onChange={(e) => setRemember(e.target.checked)}
            />
            Recordarme
          </label>

          <button
            type="button"
            className="link-like"
            onClick={() => setStatus("Función de recuperación no implementada")}
          >
            ¿Olvidaste tu contraseña?
          </button>
        </div>

        <button type="submit">Entrar</button>

        <div className="divider" aria-hidden="true">
          <span>o</span>
        </div>

        <GoogleButton
          onResult={(ok, msg) =>
            setStatus(ok ? `Google OK ✅: ${msg}` : `Google ERROR ❌: ${msg}`)
          }
          onLoginUser={(user) => {
            if (user) {
              setStatus(`Bienvenido ${user.nombre || user.email}`);
              onLoginSuccess?.(user);
            } else {
              setStatus("Error: no se recibió usuario de Google.");
            }
          }}
        />

        {status && <p className="status">{status}</p>}
        <p className="hint">* Esta sesión se mantendrá activa hasta que cierres sesión.</p>
      </form>
    </div>
  );
}
