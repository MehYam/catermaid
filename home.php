<?php
require 'utils.php';
session_start();
if (!authenticated()) {
    header('Location: login.php');
} else {
    ?>

    <!DOCTYPE html>
    <html>
        <head>
    	<meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
    	<title>Catermaid</title>
    	<style>
    	    img.kira {
    		padding: 2px;
    		border: 3px solid #fff;
    		background: #fff;
    	    }
    	    img.kira:hover {
    		border-color: #dbd5c5;
    		background: #dbd5c5;
    	    }
    	</style>
<!--	<script type="text/javascript" src="http://www.google.com/jsapi"/>  -->

        </head>
        <body>

    	<div id="header">
    	    <div align="right">
		    <strong><?php echo $_SESSION['user_email']; ?></strong>
		    <img src="images/google_whiteblue.jpg"/>
<!--		    <a href="http://www.google.com/accounts/Logout">Log out</a>  -->
    	    </div>
    	    <div id="logo">
    		<img src="images/catermaid.ico" alt="CaterMaid" />
    		<font style="color:green;font-size:200%;font-family:Tahoma;">Catermaid</font>
    	    </div>
    	    <hr>
    	    <br/>
    	</div>
        </body>
    </html>

    <?php
}
?>