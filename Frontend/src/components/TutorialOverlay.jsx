import React, { useEffect, useMemo, useRef, useState } from "react";

/**
 * TutorialOverlay
 * - Muestra pasos con foco (spotlight) sobre elementos de la UI.
 * - Se cierra con “Omitir” o al finalizar.
 * - Guarda "visto" en localStorage si setSeenKey está definido.
 *
 * Props:
 *  - steps: [{ id, selector, title, body }]
 *  - onClose: () => void
 *  - seenKey?: string (localStorage key para recordar que ya se vio)
 */
export default function TutorialOverlay({ steps, onClose, seenKey = "unitask_tutorial_seen" }) {
  const [idx, setIdx] = useState(0);
  const [rect, setRect] = useState(null);
  const backdropRef = useRef(null);

  const step = steps[idx];
  const total = steps.length;

  // Calcula el rect del elemento target
  useEffect(() => {
    const el = document.querySelector(step?.selector);
    if (!el) {
      setRect(null);
      return;
    }
    const r = el.getBoundingClientRect();
    setRect({
      x: r.left + window.scrollX,
      y: r.top + window.scrollY,
      w: r.width,
      h: r.height,
    });

    const scrollInto = () => {
      el.scrollIntoView({ block: "center", inline: "center", behavior: "smooth" });
    };
    scrollInto();

    const onResize = () => {
      const r2 = el.getBoundingClientRect();
      setRect({ x: r2.left + window.scrollX, y: r2.top + window.scrollY, w: r2.width, h: r2.height });
    };
    window.addEventListener("resize", onResize);
    window.addEventListener("scroll", onResize, { passive: true });
    return () => {
      window.removeEventListener("resize", onResize);
      window.removeEventListener("scroll", onResize);
    };
  }, [idx, step?.selector]);

  const closeAndRemember = () => {
    try { localStorage.setItem(seenKey, "1"); } catch {}
    onClose?.();
  };

  const goNext = () => {
    if (idx < total - 1) setIdx(idx + 1);
    else closeAndRemember();
  };
  const goPrev = () => setIdx((i) => Math.max(0, i - 1));

  // Posición del popover
  const popoverStyle = useMemo(() => {
    if (!rect) return { top: "20vh", left: "50%", transform: "translateX(-50%)" };
    const top = rect.y + rect.h + 12;
    const left = rect.x + rect.w / 2;
    return { top: Math.max(12, top), left, transform: "translateX(-50%)" };
  }, [rect]);

  return (
    <div className="tour__backdrop" ref={backdropRef} onClick={closeAndRemember}>
      {/* “Agujero” (spotlight) */}
      {rect && (
        <div
          className="tour__spotlight"
          style={{
            top: rect.y - 8,
            left: rect.x - 8,
            width: rect.w + 16,
            height: rect.h + 16,
          }}
          onClick={(e) => e.stopPropagation()}
        />
      )}

      {/* Popover */}
      <div
        className="tour__popover card"
        style={popoverStyle}
        onClick={(e) => e.stopPropagation()}
      >
        <div className="tour__title">{step?.title}</div>
        <div className="tour__body">{step?.body}</div>

        <div className="tour__footer">
          <div className="tour__dots">
            {steps.map((_, i) => (
              <span key={i} className={`tour__dot ${i === idx ? "active" : ""}`} />
            ))}
          </div>
          <div className="tour__actions">
            <button className="btn small outline" onClick={closeAndRemember}>Omitir</button>
            {idx > 0 && (
              <button className="btn small outline" onClick={goPrev}>Atrás</button>
            )}
            <button className="btn small" onClick={goNext}>
              {idx === total - 1 ? "¡Entendido!" : "Siguiente →"}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
