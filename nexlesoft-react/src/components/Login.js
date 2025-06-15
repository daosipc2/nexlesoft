import React, { useState, useEffect, useContext } from 'react';
import { useNavigate } from 'react-router-dom';
import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom'
import { toast } from 'react-toastify';
import { AuthContext } from '../AuthContext'
import '../login.css'

const Login = () => {
    //const signInUrl = '/backend/Users/sigin';
    const _signInUrl = 'http://nexle.seehire.us/backend/Users/sigin';

    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [errors, setErrors] = useState('');
    const [isSubmit, setIsSubmit] = useState(false);

    const usenavigate = useNavigate();
    const navigate = useNavigate();
    const { token, setToken } = useContext(AuthContext);

    useEffect(() => {
        //const token = sessionStorage.getItem('jwttoken'); 
        if (token !== null && token !== "") {
            navigate('/dashboard');
        }
    }, [navigate]);



    const ProceedLogin = (e) => {
        e.preventDefault();
        setIsSubmit(true);

        if (validate()) {
            let inputobj = {
                email: username,
                password: password,
            };

            fetch(_signInUrl, {
                method: 'POST',
                headers: { 'content-type': 'application/json' },
                body: JSON.stringify(inputobj),
            })
                .then((res) => res.json())
                .then((res) => {

                    if (res.status === 400) {
                        const errorMessages = Object.values(res.errors).flat().join('<br/> ');
                        setErrors(errorMessages);
                        // toast.error(errorMessages);
                    }
                    else if (res.status === false) {
                        console.log(res);
                        if (res.message !== null) {
                            setErrors(res.message);
                            toast.error(res.message);
                        } else {
                            toast.error('Invalid user name or password.');
                        }
                    } else {
                        // toast.success('Success');
                        let result = res.result;
                        sessionStorage.setItem('username', result.user.email);
                        sessionStorage.setItem('fullName', result.user.firstName + ' ' + result.user.lastName);
                        sessionStorage.setItem('jwttoken', result.token);
                        sessionStorage.setItem('refreshToken', result.refreshToken);
                        setToken(result.token);

                        usenavigate('/Dashboard');
                    }
                })
                .catch((err) => {
                    // toast.error(err.message);
                });
        }
    };

    const validate = () => {
        let result = true;
        if (username === '' || username === null) {
            result = false;
            // toast.warning('Please Enter Username');
        }
        if (password === '' || password === null) {
            result = false;
            // toast.warning('Please Enter Password');
        }
        return result;
    };

    return (
        <div className="auth-wrapper">
            <div className="auth-inner">
                <div className='auth-banner-left'>
                    <img src='/img/Illustrations.jpg'></img>
                </div>
                <div className='auth-right ' >
                    <div className="login-container">
                        <h4 className="welcome-title">Welcome to ReactJS Test Interview!</h4>
                        <p className="text-muted">Please sign-in to your account and start the adventure</p>
                        <form onSubmit={ProceedLogin}>
                            <div className="mb-3 text-start">
                                <label htmlFor="email" className="form-label">
                                    Email*
                                </label>
                                <input
                                    type="email"
                                    id="username"
                                    value={username}
                                    onChange={(e) => setUsername(e.target.value)}
                                    className="form-control"
                                    required
                                />
                                {isSubmit && !username && <div className="error-text">Email is not valid</div>}
                            </div>
                            <div className="mb-3 text-start">
                                <label htmlFor="password" className="form-label password-label">
                                    Password*
                                </label>
                                <a href="#" className="forgot-password">
                                    Forgot Password?
                                </a>
                                <div className="input-group">
                                    <input
                                        type="password"
                                        id="password"
                                        value={password}
                                        onChange={(e) => setPassword(e.target.value)}
                                        className="form-control"
                                        required
                                        minLength={8}
                                        maxLength={20}
                                        title="Password must be between 8 and 20 characters"
                                    />
                                    {isSubmit && !password && <div className="error-text" >Password is required</div>}
                                </div>
                            </div>
                            <div className="mb-3 form-check">
                                <input type="checkbox" className="form-check-input" id="rememberMe" />
                                <label className="form-check-label" htmlFor="rememberMe">
                                    Remember me
                                </label>
                            </div>
                            <button type="submit" className="btn btn-login mb-3">Login</button>

                            <p className="text-muted">
                                New on our platform? <Link className="text-primary" to={'/sign-up'}>
                                    Create an account
                                </Link>
                            </p>
                            <div className="social-icons mt-2">
                                <a href="#" className="text-primary">
                                    <i className="bi bi-facebook"></i>
                                </a>
                                <a href="#" className="text-info">
                                    <i className="bi bi-twitter"></i>
                                </a>
                                <a href="#" className="text-danger">
                                    <i className="bi bi-google"></i>
                                </a>
                                <a href="#" className="text-dark">
                                    <i className="bi bi-envelope"></i>
                                </a>
                            </div>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default Login;