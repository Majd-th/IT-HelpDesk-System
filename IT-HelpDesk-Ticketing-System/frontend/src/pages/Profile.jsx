import { useEffect, useState } from "react";

import Layout from "../components/Layout";

import {
    getProfile,
    updateProfile,
    changePassword
} from "../services/profileService";

import "../assets/profile.css";

function Profile() {
    const [firstName, setFirstName] = useState("");
    const [lastName, setLastName] = useState("");
    const [email, setEmail] = useState("");
    const [phoneNumber, setPhoneNumber] = useState("");
    const [role, setRole] = useState("");

    const [currentPassword, setCurrentPassword] =
        useState("");

    const [newPassword, setNewPassword] =
        useState("");

    const [confirmPassword, setConfirmPassword] =
        useState("");

    const [loading, setLoading] = useState(true);
    const [savingProfile, setSavingProfile] =
        useState(false);

    const [changingPassword, setChangingPassword] =
        useState(false);

    const [message, setMessage] = useState("");
    const [error, setError] = useState("");

    useEffect(() => {
        loadProfile();
    }, []);

    async function loadProfile() {
        try {
            setLoading(true);
            setError("");

            const data = await getProfile();

            setFirstName(data.firstName || "");
            setLastName(data.lastName || "");
            setEmail(data.email || "");
            setPhoneNumber(data.phoneNumber || "");
            setRole(data.role || "");
        } catch (requestError) {
            console.error(
                "Could not load profile:",
                requestError
            );

            setError("Could not load your profile.");
        } finally {
            setLoading(false);
        }
    }

    async function handleProfileSubmit(event) {
        event.preventDefault();

        setMessage("");
        setError("");

        if (!firstName.trim() || !lastName.trim()) {
            setError(
                "First name and last name are required."
            );

            return;
        }

        if (!email.trim()) {
            setError("Email is required.");
            return;
        }

        try {
            setSavingProfile(true);

            const updatedUser = await updateProfile({
                firstName: firstName.trim(),
                lastName: lastName.trim(),
                email: email.trim(),
                phoneNumber: phoneNumber.trim() || null
            });

            setFirstName(updatedUser.firstName);
            setLastName(updatedUser.lastName);
            setEmail(updatedUser.email);
            setPhoneNumber(
                updatedUser.phoneNumber || ""
            );
            setRole(updatedUser.role);

            const fullName =
                `${updatedUser.firstName} ` +
                `${updatedUser.lastName}`;

            localStorage.setItem(
                "fullName",
                fullName.trim()
            );

            localStorage.setItem(
                "email",
                updatedUser.email
            );

            setMessage(
                "Profile updated successfully."
            );
        } catch (requestError) {
            console.error(
                "Could not update profile:",
                requestError
            );

            setError(
                requestError.response?.data?.message ||
                "Could not update your profile."
            );
        } finally {
            setSavingProfile(false);
        }
    }

    async function handlePasswordSubmit(event) {
        event.preventDefault();

        setMessage("");
        setError("");

        if (
            !currentPassword ||
            !newPassword ||
            !confirmPassword
        ) {
            setError(
                "Complete all password fields."
            );

            return;
        }

        if (newPassword.length < 6) {
            setError(
                "The new password must contain at least 6 characters."
            );

            return;
        }

        if (newPassword !== confirmPassword) {
            setError(
                "The new passwords do not match."
            );

            return;
        }

        try {
            setChangingPassword(true);

            const response = await changePassword({
                currentPassword,
                newPassword,
                confirmPassword
            });

            setCurrentPassword("");
            setNewPassword("");
            setConfirmPassword("");

            setMessage(
                response.message ||
                "Password changed successfully."
            );
        } catch (requestError) {
            console.error(
                "Could not change password:",
                requestError
            );

            setError(
                requestError.response?.data?.message ||
                "Could not change the password."
            );
        } finally {
            setChangingPassword(false);
        }
    }

    if (loading) {
        return (
            <Layout>
                <p>Loading profile...</p>
            </Layout>
        );
    }

    const fullName =
        `${firstName} ${lastName}`.trim();

    return (
        <Layout>
            <div className="profile-page">
                <div className="profile-heading">
                    <div>
                        <h1>Profile</h1>

                        <p>
                            View and manage your account
                            information.
                        </p>
                    </div>
                </div>

                {(message || error) && (
                    <div
                        className={
                            error
                                ? "profile-alert profile-error"
                                : "profile-alert profile-success"
                        }
                    >
                        {error || message}
                    </div>
                )}

                <div className="profile-grid">
                    <section className="profile-card">
                        <div className="profile-card-header">
                            <div className="profile-avatar">
                                {firstName
                                    ? firstName
                                          .charAt(0)
                                          .toUpperCase()
                                    : "U"}
                            </div>

                            <div>
                                <h2>{fullName || "User"}</h2>

                                <span className="profile-role">
                                    {role}
                                </span>
                            </div>
                        </div>

                        <form
                            onSubmit={handleProfileSubmit}
                        >
                            <div className="profile-form-group">
                                <label htmlFor="firstName">
                                    First name
                                </label>

                                <input
                                    id="firstName"
                                    type="text"
                                    value={firstName}
                                    onChange={(event) =>
                                        setFirstName(
                                            event.target.value
                                        )
                                    }
                                />
                            </div>

                            <div className="profile-form-group">
                                <label htmlFor="lastName">
                                    Last name
                                </label>

                                <input
                                    id="lastName"
                                    type="text"
                                    value={lastName}
                                    onChange={(event) =>
                                        setLastName(
                                            event.target.value
                                        )
                                    }
                                />
                            </div>

                            <div className="profile-form-group">
                                <label htmlFor="email">
                                    Email address
                                </label>

                                <input
                                    id="email"
                                    type="email"
                                    value={email}
                                    onChange={(event) =>
                                        setEmail(
                                            event.target.value
                                        )
                                    }
                                />
                            </div>

                            <div className="profile-form-group">
                                <label htmlFor="phoneNumber">
                                    Phone number
                                </label>

                                <input
                                    id="phoneNumber"
                                    type="tel"
                                    value={phoneNumber}
                                    onChange={(event) =>
                                        setPhoneNumber(
                                            event.target.value
                                        )
                                    }
                                />
                            </div>

                            <div className="profile-form-group">
                                <label htmlFor="role">
                                    Role
                                </label>

                                <input
                                    id="role"
                                    type="text"
                                    value={role}
                                    disabled
                                />
                            </div>

                            <button
                                type="submit"
                                className="profile-save-button"
                                disabled={savingProfile}
                            >
                                {savingProfile
                                    ? "Saving..."
                                    : "Save profile"}
                            </button>
                        </form>
                    </section>

                    <section className="profile-card">
                        <div className="profile-section-title">
                            <h2>Change Password</h2>

                            <p>
                                Choose a secure password for
                                your account.
                            </p>
                        </div>

                        <form
                            onSubmit={handlePasswordSubmit}
                        >
                            <div className="profile-form-group">
                                <label htmlFor="currentPassword">
                                    Current password
                                </label>

                                <input
                                    id="currentPassword"
                                    type="password"
                                    value={currentPassword}
                                    onChange={(event) =>
                                        setCurrentPassword(
                                            event.target.value
                                        )
                                    }
                                />
                            </div>

                            <div className="profile-form-group">
                                <label htmlFor="newPassword">
                                    New password
                                </label>

                                <input
                                    id="newPassword"
                                    type="password"
                                    value={newPassword}
                                    onChange={(event) =>
                                        setNewPassword(
                                            event.target.value
                                        )
                                    }
                                />
                            </div>

                            <div className="profile-form-group">
                                <label htmlFor="confirmPassword">
                                    Confirm new password
                                </label>

                                <input
                                    id="confirmPassword"
                                    type="password"
                                    value={confirmPassword}
                                    onChange={(event) =>
                                        setConfirmPassword(
                                            event.target.value
                                        )
                                    }
                                />
                            </div>

                            <button
                                type="submit"
                                className="password-save-button"
                                disabled={changingPassword}
                            >
                                {changingPassword
                                    ? "Changing..."
                                    : "Change password"}
                            </button>
                        </form>
                    </section>
                </div>
            </div>
        </Layout>
    );
}

export default Profile;