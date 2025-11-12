import React from 'react';

import styles from './index.module.scss';

const Component = () => {
  return (
    <div className={styles.a02Login}>
      <div className={styles.frame101}>
        <p className={styles.time}>03:16</p>
        <img src="../image/mhvo9xwm-5e9rnxe.svg" className={styles.frame100} />
      </div>
      <div className={styles.frame1000006434}>
        <p className={styles.welcomeBack}>Welcome Back</p>
        <p className={styles.accessYourTripsAndTr}>
          Access your trips and transport info in seconds.
        </p>
      </div>
      <div className={styles.content}>
        <div className={styles.frame1000006360}>
          <div className={styles.inputField}>
            <p className={styles.enterEmail}>Enter Email</p>
          </div>
          <div className={styles.password}>
            <div className={styles.inputField2}>
              <p className={styles.enterEmail}>Enter password</p>
              <img src="../image/mhvo9xwm-jixve9x.svg" className={styles.eye} />
            </div>
            <div className={styles.frame1000006392}>
              <div className={styles.frame1000006363}>
                <img src="../image/mhvo9xwm-urify2c.svg" className={styles.icon} />
                <p className={styles.rememberMe}>Remember Me</p>
              </div>
              <p className={styles.forgotPassword}>Forgot password?</p>
            </div>
          </div>
          <div className={styles.button}>
            <p className={styles.signIn}>Sign In</p>
          </div>
        </div>
        <div className={styles.frame51}>
          <div className={styles.otherOption}>
            <div className={styles.dividerLine} />
            <p className={styles.or}>Or</p>
            <div className={styles.dividerLine} />
          </div>
          <div className={styles.frame48}>
            <img
              src="../image/mhvo9xwm-qsz8sur.svg"
              className={styles.frame1000001523}
            />
            <img
              src="../image/mhvo9xwm-tl1bkcd.svg"
              className={styles.frame1000001523}
            />
            <img
              src="../image/mhvo9xwm-bk0p7bn.svg"
              className={styles.frame1000001523}
            />
          </div>
          <p className={styles.donTHaveAnAccountCre3}>
            <span className={styles.donTHaveAnAccountCre}>
              Don't have an account?&nbsp;
            </span>
            <span className={styles.donTHaveAnAccountCre2}>Create Account</span>
          </p>
        </div>
      </div>
      <div className={styles.frame82}>
        <div className={styles.rectangle} />
      </div>
    </div>
  );
}

export default Component;
