import React, { useEffect, useMemo, useRef, useState } from "react";
import "../App.css";
import * as api from "./homescreen/api";
import TutorialOverlay from "../components/TutorialOverlay";

const PRIORIDADES = ["Alta", "Media", "Baja"];
const ORDENES = [
  { key: "vence", label: "Fecha de entrega" },
  { key: "prioridad", label: "Prioridad" },
];

export default function HomeScreen({ user, onLogout }) {
  const [materias, setMaterias] = useState([]);
  const [tareas, setTareas] = useState([]);
  const [etiquetas, setEtiquetas] = useState([]);
  const [status, setStatus] = useState("");
  const [recordatorios, setRecordatorios] = useState({});

  // Filtros/orden
  const [fMateria, setFMateria] = useState("all");
  const [fPrioridad, setFPrioridad] = useState("all");
  const [busqueda, setBusqueda] = useState("");
  const [orden, setOrden] = useState("vence");
  const [asc, setAsc] = useState(true);

  // Modal nueva tarea
  const [showNewTask, setShowNewTask] = useState(false);
  const [newTask, setNewTask] = useState({
    materia_id: "",
    titulo: "",
    descripcion: "",
    vence_en: "",
    prioridad: "Media",
  });

  // ======= TUTORIAL =======
  const seenKey = "unitask_tutorial_seen";
  const [showTour, setShowTour] = useState(false);

  const tourSteps = [
    {
      id: "sidebar-user",
      selector: '[data-tour="sidebar-user"]',
      title: "Tu perfil",
      body: "Aquí ves tu nombre y correo. Desde la izquierda controlás materias y progreso.",
    },
    {
      id: "materias-nueva",
      selector: '[data-tour="materias-nueva"]',
      title: "Materias",
      body: "Creá nuevas materias y hacé clic en una para filtrar sus tareas. El avance se calcula automáticamente.",
    },
    {
      id: "buscar",
      selector: '[data-tour="buscar"]',
      title: "Búsqueda rápida",
      body: "Buscá por título o descripción al vuelo.",
    },
    {
      id: "filtros",
      selector: '[data-tour="filtros"]',
      title: "Filtros y orden",
      body: "Filtrá por prioridad, elegí orden (fecha o prioridad) y alterná ascendente/descendente.",
    },
    {
      id: "nueva-tarea",
      selector: '[data-tour="nueva-tarea"]',
      title: "Nueva tarea",
      body: "Crea tareas con fecha/hora de entrega, prioridad y descripción.",
    },
    {
      id: "recordatorios",
      selector: '[data-tour="recordatorios"]',
      title: "Recordatorios",
      body: "Añadí recordatorios (minutos antes). Te notifico cuando toque.",
    },
    {
      id: "silencio",
      selector: '[data-tour="silencio"]',
      title: "Silenciar tarea",
      body: "Podés silenciar una tarea para no recibir notificaciones de ella.",
    },
  ];

  useEffect(() => {
    // Mostrar solo si nunca se ha visto
    try {
      const seen = localStorage.getItem(seenKey) === "1";
      if (!seen) setShowTour(true);
    } catch {
      // si localStorage falla, no auto-abrimos
    }
  }, []);

  // ========= CARGA INICIAL =========
  useEffect(() => {
    (async () => {
      try {
        const m = await api.listMateriasMine();
        setMaterias(m);
        if (user?.id) {
          const [t, e] = await Promise.all([
            api.listTareas(user.id),
            api.listEtiquetas(user.id),
          ]);
          setTareas(t);
          setEtiquetas(e);
        }
      } catch (err) {
        setStatus(`Error cargando datos: ${err.message || err}`);
      }
    })();
  }, [user?.id]);

  // ========= CARGAR RECORDATORIOS =========
  useEffect(() => {
    (async () => {
      if (!tareas.length) {
        setRecordatorios({});
        return;
      }
      try {
        const activos = tareas.filter((t) => !t.eliminada);
        const map = {};
        for (const t of activos) {
          const recs = await api.listRecordatorios(t.id).catch(() => []);
          map[t.id] = recs;
        }
        setRecordatorios(map);
      } catch (err) {
        setStatus(`No se pudieron cargar recordatorios: ${err.message || err}`);
      }
    })();
  }, [tareas]);

  // ========= CRUD MATERIAS =========
  const crearMateria = async () => {
    const nombre = prompt("Nombre de la materia:");
    if (!nombre) return;
    try {
      const created = await api.createMateria({ nombre });
      setMaterias((prev) => [created, ...prev]);
      setStatus("Materia creada ✅");
    } catch (err) {
      setStatus(`Error al crear materia: ${err.message || err}`);
    }
  };

  // ========= CRUD TAREAS =========
  const abrirNuevaTarea = () => {
    setNewTask({
      materia_id: materias[0]?.id || "",
      titulo: "",
      descripcion: "",
      vence_en: "",
      prioridad: "Media",
    });
    setShowNewTask(true);
  };

  const guardarNuevaTarea = async (e) => {
    e.preventDefault();
    if (!newTask.materia_id || !newTask.titulo || !newTask.vence_en) {
      setStatus("Completa materia, título y fecha/hora.");
      return;
    }
    try {
      const created = await api.createTarea(newTask);
      setTareas((prev) => [created, ...prev]);
      setRecordatorios((prev) => ({ ...prev, [created.id]: [] }));
      setShowNewTask(false);
      setStatus("Tarea creada ✅");
    } catch (err) {
      setStatus(`Error al crear tarea: ${err.message || err}`);
    }
  };

  const toggleCompletada = async (t) => {
    try {
      const updated = await api.patchTarea(t.id, { completada: !t.completada });
      setTareas((prev) => prev.map((x) => (x.id === t.id ? updated : x)));
    } catch (err) {
      setStatus(`No se pudo actualizar la tarea: ${err.message || err}`);
    }
  };

  const toggleSilenciada = async (t) => {
    try {
      const updated = await api.patchTarea(t.id, { silenciada: !t.silenciada });
      setTareas((prev) => prev.map((x) => (x.id === t.id ? updated : x)));
    } catch (err) {
      setStatus(`No se pudo cambiar el silencio: ${err.message || err}`);
    }
  };

  const eliminarTarea = async (t) => {
    if (!window.confirm("¿Eliminar esta tarea?")) return;
    try {
      await api.deleteTarea(t.id);
      setTareas((prev) => prev.filter((x) => x.id !== t.id));
      setRecordatorios((prev) => {
        const { [t.id]: _, ...rest } = prev;
        return rest;
      });
    } catch (err) {
      setStatus(`No se pudo eliminar: ${err.message || err}`);
    }
  };

  // ========= CRUD RECORDATORIOS =========
  const crearRecordatorio = async (tarea) => {
    const min = prompt("¿Cuántos minutos antes enviar el recordatorio?");
    const n = parseInt(min);
    if (isNaN(n) || n <= 0) return alert("Ingresa un número válido (> 0).");
    try {
      const created = await api.createRecordatorio(tarea.id, n);
      setRecordatorios((prev) => ({
        ...prev,
        [tarea.id]: [...(prev[tarea.id] || []), created],
      }));
      setStatus(`Recordatorio ${n} min creado ✅`);
    } catch (err) {
      setStatus(`Error al crear recordatorio: ${err.message || err}`);
    }
  };

  const toggleRecordatorio = async (tareaId, rec) => {
    try {
      const upd = await api.patchRecordatorio(rec.id, { activo: !rec.activo });
      setRecordatorios((prev) => ({
        ...prev,
        [tareaId]: prev[tareaId].map((r) => (r.id === rec.id ? { ...r, ...upd } : r)),
      }));
    } catch (err) {
      setStatus(`Error al actualizar recordatorio: ${err.message || err}`);
    }
  };

  const eliminarRecordatorio = async (tareaId, rec) => {
    if (!window.confirm("¿Eliminar este recordatorio?")) return;
    try {
      await api.deleteRecordatorio(rec.id);
      setRecordatorios((prev) => ({
        ...prev,
        [tareaId]: prev[tareaId].filter((r) => r.id !== rec.id),
      }));
    } catch (err) {
      setStatus(`Error al eliminar recordatorio: ${err.message || err}`);
    }
  };

  // ========= FILTROS / ORDEN =========
  const tareasFiltradas = useMemo(() => {
    let result = [...tareas];
    if (fMateria !== "all") result = result.filter((t) => t.materia_id === fMateria);
    if (fPrioridad !== "all") result = result.filter((t) => t.prioridad === fPrioridad);
    if (busqueda.trim()) {
      const q = busqueda.toLowerCase();
      result = result.filter(
        (t) =>
          (t.titulo || "").toLowerCase().includes(q) ||
          (t.descripcion || "").toLowerCase().includes(q)
      );
    }
    result.sort((a, b) => {
      if (orden === "vence") {
        const va = new Date(a.vence_en).getTime();
        const vb = new Date(b.vence_en).getTime();
        return asc ? va - vb : vb - va;
      }
      if (orden === "prioridad") {
        const rank = { Alta: 3, Media: 2, Baja: 1 };
        const ra = rank[a.prioridad] || 0;
        const rb = rank[b.prioridad] || 0;
        return asc ? rb - ra : ra - rb;
      }
      return 0;
    });
    return result;
  }, [tareas, fMateria, fPrioridad, busqueda, orden, asc]);

  // ========= PROGRESO POR MATERIA =========
  const progresoMateria = useMemo(() => {
    const map = {};
    for (const m of materias) map[m.id] = { total: 0, done: 0 };
    for (const t of tareas) {
      if (!map[t.materia_id]) continue;
      map[t.materia_id].total++;
      if (t.completada) map[t.materia_id].done++;
    }
    return map;
  }, [materias, tareas]);

  // ========= NOTIFICACIONES =========
  const pollRef = useRef(null);
  useEffect(() => {
    if (!("Notification" in window)) return;
    if (Notification.permission === "default") {
      Notification.requestPermission().catch(() => {});
    }
  }, []);

  const notifiedKey = (id) => `ut_notif_${id}`;
  const wasNotified = (id) => {
    try { return localStorage.getItem(notifiedKey(id)) === "1"; } catch { return false; }
  };
  const markNotified = (id) => {
    try { localStorage.setItem(notifiedKey(id), "1"); } catch {}
  };
  const showNotification = (t, mins) => {
    const title = `⏰ ${t.titulo}`;
    const body = `Vence ${new Date(t.vence_en).toLocaleTimeString()} • ${mins} min antes`;
    if ("Notification" in window && Notification.permission === "granted") {
      try { new Notification(title, { body }); } catch {}
    } else {
      setStatus(`Recordatorio: ${t.titulo} (${mins} min antes)`);
    }
  };

  useEffect(() => {
    const tick = () => {
      const now = Date.now();
      for (const t of tareas) {
        if (t.eliminada || t.completada || t.silenciada) continue;
        for (const r of recordatorios[t.id] || []) {
          if (!r.activo || r.enviado_en || wasNotified(r.id)) continue;
          const remindAt = new Date(t.vence_en).getTime() - r.minutos_antes * 60000;
          if (now >= remindAt) {
            showNotification(t, r.minutos_antes);
            markNotified(r.id);
          }
        }
      }
    };
    if (pollRef.current) clearInterval(pollRef.current);
    pollRef.current = setInterval(tick, 15000);
    tick();
    return () => {
      if (pollRef.current) clearInterval(pollRef.current);
      pollRef.current = null;
    };
  }, [tareas, recordatorios]);

  const PrioBadge = ({ p }) => {
    const k = p === "Alta" ? "prio--high" : p === "Baja" ? "prio--low" : "prio--mid";
    return <span className={`prio ${k}`}>{p || "Media"}</span>;
  };

  const BellIcon = ({ active }) => (
    <button
      className={`icon ${active ? "active" : ""}`}
      title={active ? "Silenciada" : "Activa"}
      data-tour="silencio"
    >
      {active ? "🔕" : "🔔"}
    </button>
  );

  const handleLogout = () => {
    localStorage.removeItem("user");
    localStorage.removeItem("auth_token");
    onLogout?.();
  };

  return (
    <div className="home">
      {/* ==== SIDEBAR ==== */}
      <aside className="sidebar">
        <div className="brand">
          <div className="brand__title">UniTask</div>
          <div className="brand__user" data-tour="sidebar-user">
            <div className="badge">{(user?.nombre || user?.email || "U").slice(0,1).toUpperCase()}</div>
            <div>
              <div style={{ fontWeight: 600 }}>{user?.nombre || "Usuario"}</div>
              <div className="muted" style={{ fontSize: 12 }}>{user?.email}</div>
            </div>
          </div>
        </div>

        <section className="sidebar__section">
          <div className="section__title">
            <span>Materias</span>
            <button
              className="btn small outline"
              onClick={crearMateria}
              data-tour="materias-nueva"
            >
              + Nueva
            </button>
          </div>

          {!materias.length && <div className="empty">No tienes materias aún.</div>}

          {materias.map((m) => {
            const prog = progresoMateria[m.id] || { total: 0, done: 0 };
            const pct = prog.total ? Math.round((prog.done / prog.total) * 100) : 0;
            return (
              <button
                key={m.id}
                className={`pill ${fMateria === m.id ? "active" : ""}`}
                onClick={() => setFMateria((cur) => (cur === m.id ? "all" : m.id))}
              >
                <div>
                  <div style={{ fontWeight: 600 }}>{m.nombre}</div>
                  <div className="muted" style={{ fontSize: 12 }}>
                    {prog.done}/{prog.total} completadas
                    <span className="progressbar"><span style={{ width: `${pct}%` }} /></span>
                  </div>
                </div>
              </button>
            );
          })}
        </section>

        <div className="sidebar__footer" style={{ display: "grid", gap: 8 }}>
          <button className="btn outline" onClick={() => setShowTour(true)}>
            Ver tutorial
          </button>
          <button className="btn outline" onClick={handleLogout}>Cerrar sesión</button>
        </div>
      </aside>

      {/* ==== MAIN ==== */}
      <main className="main">
        <div className="toolbar">
          <div className="search" data-tour="buscar">
            <input
              type="search"
              placeholder="Buscar (título o descripción)…"
              value={busqueda}
              onChange={(e) => setBusqueda(e.target.value)}
            />
          </div>

          <div className="filters" data-tour="filtros">
            <select value={fPrioridad} onChange={(e) => setFPrioridad(e.target.value)}>
              <option value="all">Todas las prioridades</option>
              {PRIORIDADES.map((p) => <option key={p} value={p}>{p}</option>)}
            </select>

            <select value={orden} onChange={(e) => setOrden(e.target.value)}>
              {ORDENES.map((o) => <option key={o.key} value={o.key}>{o.label}</option>)}
            </select>

            <label className="check">
              <input type="checkbox" checked={asc} onChange={() => setAsc((a) => !a)} />
              Ascendente
            </label>
          </div>

          <button
            className="btn"
            onClick={abrirNuevaTarea}
            style={{ marginLeft: "auto" }}
            data-tour="nueva-tarea"
          >
            + Nueva tarea
          </button>
        </div>

        <div className="tasklist">
          {!tareasFiltradas.length ? (
            <div className="card empty">No hay tareas.</div>
          ) : (
            tareasFiltradas.map((t) => {
              const materia = materias.find((m) => m.id === t.materia_id);
              return (
                <div key={t.id} className={`task ${t.completada ? "done" : ""}`}>
                  <div className="task__left">
                    <input type="checkbox" checked={!!t.completada} onChange={() => toggleCompletada(t)} />
                  </div>

                  <div>
                    <div className="task__header">
                      <strong>{t.titulo}</strong>
                      <PrioBadge p={PRIORIDADES.includes(t.prioridad) ? t.prioridad : "Media"} />
                      <span className="chip">vence {new Date(t.vence_en).toLocaleString()}</span>
                    </div>

                    <div className="task__meta">
                      <span className="chip">{materia?.nombre || "Sin materia"}</span>
                      {!!t.descripcion && <div className="muted">{t.descripcion}</div>}
                    </div>

                    {/* ===== Recordatorios de esta tarea ===== */}
                    <div
                      className="recordatorios"
                      style={{ marginTop: 8 }}
                      data-tour="recordatorios"
                    >
                      <b>Recordatorios:</b>{" "}
                      {(recordatorios[t.id] || []).length === 0 && <span className="muted">ninguno</span>}
                      <div style={{ display: "grid", gap: 6, marginTop: 6 }}>
                        {(recordatorios[t.id] || []).map((r) => (
                          <div key={r.id} className="rec-item" style={{ display: "flex", gap: 8, alignItems: "center" }}>
                            <span>⏰ {r.minutos_antes} min</span>
                            <button className="btn small" onClick={() => toggleRecordatorio(t.id, r)}>
                              {r.activo ? "Desactivar" : "Activar"}
                            </button>
                            <button className="btn small outline" onClick={() => eliminarRecordatorio(t.id, r)}>
                              Eliminar
                            </button>
                            {r.enviado_en && <span className="muted" style={{ fontSize: 12 }}>
                              enviado {new Date(r.enviado_en).toLocaleString()}
                            </span>}
                          </div>
                        ))}
                      </div>
                      <button className="btn small outline" onClick={() => crearRecordatorio(t)} style={{ marginTop: 6 }}>
                        + Añadir recordatorio
                      </button>
                    </div>
                  </div>

                  <div className="task__actions" style={{ display: "grid", gap: 8 }}>
                    <div onClick={() => toggleSilenciada(t)}><BellIcon active={t.silenciada} /></div>
                    <button className="btn small outline" onClick={() => eliminarTarea(t)}>Eliminar tarea</button>
                  </div>
                </div>
              );
            })
          )}
        </div>

        {status && <div className="status" style={{ margin: "0 18px 18px" }}>{status}</div>}
      </main>

      {/* ==== MODAL NUEVA TAREA ==== */}
      {showNewTask && (
        <div
          className="modal"
          style={{
            position: "fixed", inset: 0, display: "grid", placeItems: "center",
            background: "rgba(2,6,23,.6)", backdropFilter: "blur(4px)", zIndex: 50, padding: 16,
          }}
          onClick={() => setShowNewTask(false)}
        >
          <form
            className="card"
            onClick={(e) => e.stopPropagation()}
            onSubmit={guardarNuevaTarea}
            style={{ maxWidth: 560, width: "100%" }}
          >
            <h3 style={{ marginTop: 0 }}>Nueva tarea</h3>

            <label>Materia</label>
            <select
              value={newTask.materia_id}
              onChange={(e) => setNewTask((s) => ({ ...s, materia_id: e.target.value }))}
              required
            >
              <option value="" disabled>Selecciona una materia</option>
              {materias.map((m) => <option key={m.id} value={m.id}>{m.nombre}</option>)}
            </select>

            <label style={{ marginTop: 8 }}>Título</label>
            <input value={newTask.titulo} onChange={(e) => setNewTask((s) => ({ ...s, titulo: e.target.value }))} required />

            <label style={{ marginTop: 8 }}>Descripción</label>
            <textarea rows={3} value={newTask.descripcion} onChange={(e) => setNewTask((s) => ({ ...s, descripcion: e.target.value }))} />

            <label style={{ marginTop: 8 }}>Fecha/Hora de entrega</label>
            <input type="datetime-local" value={newTask.vence_en} onChange={(e) => setNewTask((s) => ({ ...s, vence_en: e.target.value }))} required />

            <label style={{ marginTop: 8 }}>Prioridad</label>
            <select value={newTask.prioridad} onChange={(e) => setNewTask((s) => ({ ...s, prioridad: e.target.value }))}>
              {PRIORIDADES.map((p) => <option key={p} value={p}>{p}</option>)}
            </select>

            <div className="row" style={{ marginTop: 14, justifyContent: "flex-end", gap: 8 }}>
              <button type="button" className="btn outline" onClick={() => setShowNewTask(false)}>Cancelar</button>
              <button type="submit" className="btn">Crear</button>
            </div>
          </form>
        </div>
      )}

      {/* ==== TUTORIAL OVERLAY ==== */}
      {showTour && (
        <TutorialOverlay
          steps={tourSteps}
          onClose={() => setShowTour(false)}
          seenKey={seenKey}
        />
      )}
    </div>
  );
}
