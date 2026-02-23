function apiBase() {
  return '';
}

function saveAuth(auth) {
  localStorage.setItem('foodie_auth', JSON.stringify(auth));
}

function loadAuth() {
  const raw = localStorage.getItem('foodie_auth');
  if (!raw) return null;
  try { return JSON.parse(raw); } catch { return null; }
}

function clearAuth() {
  localStorage.removeItem('foodie_auth');
}

function authHeader() {
  const auth = loadAuth();
  if (!auth || !auth.token || auth.guest === true) return {};
  return { Authorization: `Bearer ${auth.token}` };
}

async function postJson(path, body, headers = {}) {
  const res = await fetch(`${apiBase()}${path}`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      ...headers
    },
    body: JSON.stringify(body)
  });

  const text = await res.text();
  let json = null;
  if (text) {
    try { json = JSON.parse(text); } catch { json = { message: text }; }
  }

  if (!res.ok) {
    const msg = (json && (json.message || json.title)) || text || `HTTP ${res.status}`;
    throw new Error(msg);
  }

  return json;
}

function requireAuth(redirect = '/index.html') {
  const auth = loadAuth();
  if (!auth || (!auth.token && auth.guest !== true)) {
    location.href = redirect;
    return null;
  }
  return auth;
}

function isGuestAuth(auth) {
  return Boolean(auth && auth.guest === true);
}

function text(el, value, cls) {
  if (!el) return;
  el.textContent = value || '';
  el.className = `notice${cls ? ` ${cls}` : ''}`;
}

function ensureToastWrap() {
  let wrap = document.getElementById('toast-wrap');
  if (!wrap) {
    wrap = document.createElement('div');
    wrap.id = 'toast-wrap';
    wrap.className = 'toast-wrap';
    document.body.appendChild(wrap);
  }
  return wrap;
}

function toast(message, cls = '') {
  const wrap = ensureToastWrap();
  const t = document.createElement('div');
  t.className = `toast${cls ? ` ${cls}` : ''}`;
  t.textContent = message || 'Unknown message';
  wrap.appendChild(t);
  setTimeout(() => t.remove(), 3200);
}

function parseQuery() {
  const p = new URLSearchParams(location.search);
  return {
    lat: Number(p.get('lat')),
    lng: Number(p.get('lng'))
  };
}

function saveLastLocation(lat, lng) {
  localStorage.setItem('foodie_last_location', JSON.stringify({ lat, lng }));
}

function loadLastLocation() {
  const raw = localStorage.getItem('foodie_last_location');
  if (!raw) return null;
  try {
    const obj = JSON.parse(raw);
    if (!Number.isFinite(obj?.lat) || !Number.isFinite(obj?.lng)) return null;
    return obj;
  } catch {
    return null;
  }
}

function distanceValueToKm(value) {
  switch (value) {
    case '1km_less': return 1;
    case '1to5km': return 5;
    case '5to10km': return 10;
    case '10km_more': return 20;
    default: return 5;
  }
}

function priceToId(value) {
  switch (value) {
    case '100_less': return 1;
    case '100_199': return 2;
    case '200_299': return 3;
    case '300_399': return 4;
    case '400_499': return 5;
    case '500_more': return 6;
    default: return 1;
  }
}

function eatModeToId(value) {
  switch (value) {
    case 'alone': return 1;
    case 'friend': return 2;
    case 'couple': return 3;
    case 'family': return 4;
    default: return 1;
  }
}

function hungerToId(value) {
  return value === 'eat_less' ? 2 : 1;
}

function hotColdToId(value) {
  return value === 'cold' ? 2 : 1;
}

function tasteToId(value) {
  return value === 'sweety' ? 2 : 1;
}

window.FC = {
  saveAuth,
  loadAuth,
  clearAuth,
  postJson,
  authHeader,
  requireAuth,
  isGuestAuth,
  text,
  parseQuery,
  saveLastLocation,
  loadLastLocation,
  distanceValueToKm,
  priceToId,
  eatModeToId,
  hungerToId,
  hotColdToId,
  tasteToId,
  toast
};

window.addEventListener('error', (e) => {
  if (!e || !e.message) return;
  toast(e.message, 'error');
});

window.addEventListener('unhandledrejection', (e) => {
  const msg = e?.reason?.message || e?.reason || 'Unhandled error';
  toast(String(msg), 'error');
});
