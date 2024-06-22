import logo from './logo.svg';
import './App.css';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.css';

import Header from './components/header';
import Home from './components/home';
import Register from './components/register';
import Login from './components/login';

function App() {
  return (
    <Router>
      <div className="app">
      <Header/>
        <div className="content">
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
        </Routes>
        </div>
      </div>
    </Router>
  );
}

export default App;
