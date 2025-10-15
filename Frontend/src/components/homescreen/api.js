// API client para HomeScreen
const API = "http://localhost:5000";

function authHeaders() {
  const t = localStorage.getItem("auth_token");
  return t ? { Authorization: `Bearer ${t}` } : {};
}

/** ================== MATERIAS ================== **/
export async function listMateriasMine() {
  const res = await fetch(`${API}/materias/mias`, { headers: { ...authHeaders() } });
  if (!res.ok) throw new Error(await safeText(res));
  return res.json();
}

export async function createMateria({ nombre }) {
  const res = await fetch(`${API}/materias`, {
    method: "POST",
    headers: { "Content-Type": "application/json", ...authHeaders() },
    body: JSON.stringify({ nombre }),
  });
  if (!res.ok) throw new Error(await safeText(res));
  return res.json();
}

// (Opcional si tu backend lo tiene)
export async function patchMateria(id, patch) {
  const res = await fetch(`${API}/materias/${id}`, {
    method: "PATCH",
    headers: { "Content-Type": "application/json", ...authHeaders() },
    body: JSON.stringify(patch),
  });
  if (!res.ok) throw new Error(await safeText(res));
  return res.json();
}

// (Opcional si tu backend lo tiene)
export async function deleteMateria(id) {
  const res = await fetch(`${API}/materias/${id}`, {
    method: "DELETE",
    headers: { ...authHeaders() },
  });
  if (!res.ok) throw new Error(await safeText(res));
  return res.json();
}

/** ================== TAREAS ================== **/
export async function listTareas(usuario_id) {
  const res = await fetch(`${API}/tareas?usuario_id=${encodeURIComponent(usuario_id)}`, {
    headers: { ...authHeaders() },
  });
  if (!res.ok) throw new Error(await safeText(res));
  return res.json();
}

export async function createTarea({ materia_id, titulo, descripcion, vence_en, prioridad }) {
  const res = await fetch(`${API}/tareas`, {
    method: "POST",
    headers: { "Content-Type": "application/json", ...authHeaders() },
    body: JSON.stringify({ materia_id, titulo, descripcion, vence_en, prioridad }),
  });
  if (!res.ok) throw new Error(await safeText(res));
  return res.json();
}

export async function patchTarea(id, patch) {
  const res = await fetch(`${API}/tareas/${id}`, {
    method: "PATCH",
    headers: { "Content-Type": "application/json", ...authHeaders() },
    body: JSON.stringify(patch),
  });
  if (!res.ok) throw new Error(await safeText(res));
  return res.json();
}

export async function deleteTarea(id) {
  const res = await fetch(`${API}/tareas/${id}`, {
    method: "DELETE",
    headers: { ...authHeaders() },
  });
  if (!res.ok) throw new Error(await safeText(res));
  return res.json();
}

/** ================== ETIQUETAS ================== **/
export async function listEtiquetas(usuario_id) {
  const res = await fetch(`${API}/etiquetas?usuario_id=${encodeURIComponent(usuario_id)}`, {
    headers: { ...authHeaders() },
  });
  if (!res.ok) throw new Error(await safeText(res));
  return res.json();
}

// (Si luego agregas endpoints de crear/editar/eliminar etiquetas)
// export async function createEtiqueta({ nombre, color_hex }) { ... }
// export async function patchEtiqueta(id, patch) { ... }
// export async function deleteEtiqueta(id) { ... }

/** ================== RECORDATORIOS ================== **/
// Usa el controlador nuevo que hiciste:
// GET     /tareas/{tareaId}/recordatorios
// POST    /tareas/{tareaId}/recordatorios   { minutos_antes }
// PATCH   /recordatorios/{id}               { activo }
// DELETE  /recordatorios/{id}

export async function listRecordatorios(tarea_id) {
  const res = await fetch(`${API}/tareas/${tarea_id}/recordatorios`, {
    headers: { ...authHeaders() },
  });
  if (!res.ok) throw new Error(await safeText(res));
  return res.json();
}

export async function createRecordatorio(tarea_id, minutos_antes) {
  const res = await fetch(`${API}/tareas/${tarea_id}/recordatorios`, {
    method: "POST",
    headers: { "Content-Type": "application/json", ...authHeaders() },
    body: JSON.stringify({ minutos_antes }),
  });
  if (!res.ok) throw new Error(await safeText(res));
  return res.json();
}

export async function patchRecordatorio(id, patch) {
  const res = await fetch(`${API}/recordatorios/${id}`, {
    method: "PATCH",
    headers: { "Content-Type": "application/json", ...authHeaders() },
    body: JSON.stringify(patch),
  });
  if (!res.ok) throw new Error(await safeText(res));
  return res.json();
}

export async function deleteRecordatorio(id) {
  const res = await fetch(`${API}/recordatorios/${id}`, {
    method: "DELETE",
    headers: { ...authHeaders() },
  });
  if (!res.ok) throw new Error(await safeText(res));
  return res.json();
}

/** ================== Utils ================== **/
async function safeText(res) {
  try { return await res.text(); } catch { return `HTTP ${res.status}`; }
}
