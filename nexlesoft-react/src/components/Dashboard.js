import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom'
import "../dashboard.css"

const Dashboard = () => {
    const navigate = useNavigate();

    useEffect(() => {
        const token = sessionStorage.getItem('jwttoken');
        if (!token) {
            navigate('/sign-in');
        }
    }, [navigate]);

    return (
        <div className="auth-wrapper">
            <div className="auth-inner">

                <div className="container mt-3">
                    <h1>Welcome to Dashboard</h1>
                    <div>
                        <img alt="" src='/img/Illustration-2.svg' />
                    </div>
                </div>
            </div>
        </div>
    );
}

export default Dashboard;