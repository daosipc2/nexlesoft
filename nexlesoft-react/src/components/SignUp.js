import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom'

import '../signup.css'

const SignUp = () => {

    // const signInUrl = "/backend/Users/signup";
    const _signupUrl = 'http://nexle.seehire.us/backend/Users/signup';
    const initialValues = {
        agreedToTerms: false,
        firstName: '',
        lastName: '',
        email: '',
        password: '',
    };

    const [formData, setFormData] = useState(initialValues);
    const [isSubmit, setIsSubmit] = useState(false);
    const [errors, setErrors] = useState('');

    const navigate = useNavigate();

    const IsValidate = () => {
        let isValid = true;
        if (!formData.agreedToTerms) {
            isValid = false;
        }
        else if (/^[a-zA-Z0-9]+@[a-zA-Z0-9]+\.[A-Za-z]+$/.test(formData.email)) {
            // valid email
        } else {
            isValid = false;
            toast.warning('Please enter the valid email')
        }

        return isValid;
    }
    function handleCheckbox(event) {
        setFormData({ ...formData, [event.target.name]: event.target.checked })
    }

    function handleInput(event) {
        setFormData({ ...formData, [event.target.name]: event.target.value })
    }
    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData((prev) => ({
            ...prev,
            [name]: value,
        }));
    };

    const handlesubmit = (e) => {
        e.preventDefault();
        setIsSubmit(true);

        if (IsValidate()) {
            fetch(_signupUrl, {
                method: "POST",
                headers: { 'content-type': 'application/json' },
                body: JSON.stringify(formData)
            })
                .then((res) => {
                    return res.json();
                })
                .then((res) => {
                    if (res.status === 400) {
                        // will succeed if the server will always respond with JSON with a 400 response                   

                        const errorMessages = Object.values(res.errors).flat().join('<br/> ');
                        setErrors(errorMessages);
                        toast.error(errorMessages);
                    }
                    else if (res.status == false) {
                        setErrors(res.message);
                        toast.error(res.message);
                    }
                    else {
                        toast.success('Signup successfully.')
                        navigate('/');
                    }
                }).catch((err) => {
                    toast.error('Failed :' + err.message);
                });
        }
    }
    return (
        <div className="auth-wrapper">
            <div className="auth-inner">
                <div className='auth-banner-left'>
                    <img src='/img/Group 90.png'></img>
                </div>
                <div className='auth-right ' >
                    <div className="signup-container">
                        <h2 className="text-primary">Adventure starts here</h2>
                        <p className="text-muted">Make your app management easy and fun!</p>
                        <form onSubmit={handlesubmit}>
                            <div className="mb-3 text-start">
                                <label htmlFor="firstName" className="form-label">Firstname*</label>

                                <input
                                    name="firstName"
                                    id="firstName"
                                    value={formData.firstName}
                                    onChange={handleChange}
                                    className="form-control"
                                    required
                                />
                                {isSubmit && !formData.firstName && <div className="error-text">Firstname is required</div>}
                            </div>
                            <div className="mb-3 text-start">
                                <label htmlFor="lastName" className="form-label">Lastname*</label>
                                <input
                                    name="lastName"
                                    value={formData.lastName}
                                    onChange={handleChange}
                                    className="form-control"
                                    required
                                />
                                {isSubmit && !formData.lastName && <div className="error-text">Lastname is required</div>}
                            </div>
                            <div className="mb-3 text-start">
                                <label htmlFor="email" className="form-label">Email*</label>
                                <input
                                    name="email"
                                    value={formData.email}
                                    onChange={handleChange}
                                    className="form-control"
                                    required
                                />
                                {isSubmit && !formData.email && <div className="error-text">Email is required</div>}
                            </div>
                            <div className="mb-3 text-start">
                                <label htmlFor="password" className="form-label">Password*</label>
                                <input
                                    name="password"
                                    value={formData.password}
                                    onChange={handleChange}
                                    type="password"
                                    className="form-control"
                                    required
                                    minLength={8}
                                    maxLength={20}
                                    title="Password must be between 8 and 20 characters"
                                />
                                {isSubmit && !formData.password && <div className="error-text" >Password is required</div>}
                            </div>
                            <div className="mb-3 form-check">
                                <input
                                    name="agreedToTerms"
                                    id="agreedToTerms"
                                    type="checkbox"
                                    className="form-check-input"
                                    checked={formData.agreedToTerms}
                                    onChange={(e) => {
                                        handleCheckbox(e)
                                    }}
                                />
                                <label className="form-check-label" htmlFor="agreedToTerms">I agree to privacy policy & terms</label>
                                {isSubmit && !formData.agreedToTerms && <div className="error-text" >Please agree to privacy policy & terms</div>}
                            </div>
                            <button type="submit" className="btn btn-signup text-white mb-3">Sign Up</button>

                            <p className="text-muted">Already have an account? <Link to={'/sign-in'}>
                                Sign in instead
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
    )
}

export default SignUp;