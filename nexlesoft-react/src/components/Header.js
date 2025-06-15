import React, { useState, useEffect, useContext } from 'react';
import { Navbar, NavbarBrand, Nav, NavItem, Button, Collapse } from 'reactstrap';
import { useNavigate } from "react-router-dom";
import { toast } from "react-toastify";
import { AuthContext } from '../AuthContext'

import 'bootstrap/dist/css/bootstrap.min.css';
import '../Header.css';

const Header = () => {
    //const signoutUrl = "http://localhost:63924/Users/signout";
    const _signoutUrl = 'http://nexle.seehire.us/backend/Users/signout';
    const navigate = useNavigate();
    const { token, setToken } = useContext(AuthContext);    
    const [isOpen, setIsOpen] = useState(false);
    const [errors, setErrors] = useState('');

    const toggle = () => {
        setIsOpen(!isOpen);
    };

    const handleLogout = (e) => {
        const jwtToken = sessionStorage.getItem('jwttoken');
        const refreshToken = sessionStorage.getItem('refreshToken');
        let data ={RefreshToken:refreshToken};

        fetch(_signoutUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${jwtToken}`,
            },
            body: JSON.stringify(data),
        }).then((res) => {
            console.log(res)

            if (res.status === 500) {
                // will succeed if the server will always respond with JSON with a 400 response                   

                const errorMessages = "Bad request!";
                setErrors(errorMessages);
                toast.error(errorMessages);
            }
            else if (res.status === 204) {
                //toast.success('Logout...');                
                sessionStorage.removeItem('username');
                sessionStorage.removeItem('fullName');
                sessionStorage.removeItem('jwttoken');
                sessionStorage.removeItem('refreshToken');                
                setToken(null);
                navigate('/sign-in');
            }
        }).catch((err) => {
            toast.error(err.message);
        });
    }
    return (
        <Navbar color="light" light expand="md" className="header">
            <NavbarBrand href="/">My React App</NavbarBrand>
            <Nav className="ml-auto" navbar>
                <Collapse isOpen={isOpen} className='dropdown2' >
                    <Nav >
                        <NavItem>
                            <a onClick={handleLogout} href='#'>
                                <img src='/img/Group 93.png'></img>
                            </a>
                        </NavItem>
                    </Nav>
                </Collapse>
                <NavItem className="user-info" onClick={toggle}>
                    <img
                        src="/img/premium_photo-1689977927774-401b12d137d6.jpg"
                        alt="User Avatar"
                        className="user-avatar"
                    />
                    <div>
                        <span className="user-name" >John Doe</span>
                        <span className="user-status">Available</span>
                    </div>
                </NavItem>


            </Nav>

        </Navbar>
    );
};

export default Header;