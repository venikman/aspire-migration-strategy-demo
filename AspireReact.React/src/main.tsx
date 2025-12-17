import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import App from './App.tsx'
import './index.css'
import { initializeTelemetry } from './telemetry'

// Initialize OpenTelemetry BEFORE rendering the app
// This ensures all fetch calls and user interactions are traced
initializeTelemetry()

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <App />
  </StrictMode>,
)
