import { useEffect, useRef } from "react";

const API_BASE = "http://localhost:5000"

export default function GoogleButton({ onResult, onLoginUser }) {
  const btnRef = useRef(null);

  useEffect(() => {
    /* global google */
    if (!window.google) return;

    google.accounts.id.initialize({
      client_id: "tucliente.apps.googleusercontent.com",
      callback: async (response) => {
        try {
          const res = await fetch(`${API_BASE}/auth/google`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
           
            body: JSON.stringify({ credential: response.credential })
          });

          const text = await res.text();
          let payload = null;
          try { payload = JSON.parse(text); } catch { /* por si hay texto plano */ }

          if (!res.ok) {
            const msg = payload?.message || text || `HTTP ${res.status}`;
            throw new Error(msg);
          }

          const user = payload?.user;
          const token = payload?.token;

          if (user) {
            
            localStorage.setItem("user", JSON.stringify(user));
            if (token) localStorage.setItem("auth_token", token);

            onLoginUser?.(user);
            onResult?.(true, "Login OK");
          } else {
            onResult?.(false, "Respuesta sin usuario");
          }
        } catch (e) {
          onResult?.(false, e?.message || "Fallo en llamada a API");
        }
      }
    });

    google.accounts.id.renderButton(btnRef.current, {
      theme: "filled_black",
      size: "large",
      shape: "pill",
      text: "signin_with",
      logo_alignment: "left"
    });
  }, [onResult, onLoginUser]);

  return <div ref={btnRef} />;
}
