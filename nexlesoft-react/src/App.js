import logo from './logo.svg';
import React, { useEffect, useState } from 'react';
import './App.css';
import '../node_modules/bootstrap/dist/css/bootstrap.min.css'
import { BrowserRouter as Router, Routes, Route, Link, useLocation } from 'react-router-dom'
import { ToastContainer } from 'react-toastify';
import Login from './components/Login'
import SignUp from './components/SignUp'
import Dashboard from './components/Dashboard'
import Header from './components/Header';
import { AuthProvider, AuthContext } from './AuthContext';


function App() {
    return (
        <AuthProvider>
            <Router>
                <div className="App">
                    <ToastContainer theme='colored' position='top-center'></ToastContainer>
                    <AuthContext.Consumer>
                        {({ token }) => (token && <Header />)}
                    </AuthContext.Consumer>
                    <Routes>
                        <Route exact path="/" element={<Login />} />
                        <Route path="/sign-in" element={<Login />} />
                        <Route path="/sign-up" element={<SignUp />} />
                        <Route path="/dashboard" element={<Dashboard />} />
                    </Routes>

                </div>
            </Router>
        </AuthProvider>
    )
}

export default App
