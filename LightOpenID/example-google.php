<?php
# Logging in with Google accounts requires setting special identity, so this example shows how to do it.
require 'openid.php';
try {
    $openid = new LightOpenID;
    if (!$openid->mode) {
        if (isset($_GET['login'])) {
            $openid->required = array('contact/email');
            $openid->identity = 'https://www.google.com/accounts/o8/id';
            header('Location: ' . $openid->authUrl());
        }
        ?>
        <form action="?login" method="post">
            <button>Login with Google</button>
        </form>
        <?php
    } elseif ($openid->mode == 'cancel') {
        echo 'User has canceled authentication!';
    } elseif($openid->validate()){
        $allowedUsers = array('kerbumble@gmail.com', 'cdn.kai@gmail.com', 'briantu81@gmail.com');
        $attrs = $openid->getAttributes();
        $userEmail = $attrs['contact/email'];
        if (in_array($userEmail, $allowedUsers)) {
            echo 'User ' . $openid->identity . ' has logged in.';
            echo '<br/>Email: ' . $userEmail;
        }
        else {
            echo 'Access Denied to: ' . $userEmail;
        }
    } else {
        echo 'User ' . ($openid->validate() ? $openid->identity . ' has ' : 'has not ') . 'logged in.';
    }
} catch (ErrorException $e) {
    echo $e->getMessage();
}
?>