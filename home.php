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
        </head>
        <body>
    	<script src="jQuery/jquery.js"></script>
	<script src="jQuery/jquery.cookie.js"></script>

    	<div id="header">
    	    <div align="right">
    		<strong><?php echo $_SESSION['user_email']; ?></strong>
    		<img src="images/google_whiteblue.jpg"/>
		<div id="signout"><a href="http://www.google.com/accounts/Logout">Log out</a> </div>
    	    </div>
    	    <div id="logo">
    		<img src="images/catermaid.ico" alt="CaterMaid" />
    		<font style="color:green;font-size:200%;font-family:Tahoma;">Catermaid</font>
    	    </div>
    	    <hr>
    	    <br/>
    	</div>
    	<br/>

    	<script>
	    //document ready event handler
    	    $(document).ready(function(){

		//click handler for "Log out" link
		//delete the "openid" cookie when a user logs out
    		$("#signout a").click(function()
		{
		    $.cookie("openid", null);
    		});
    	    });
    	</script>
        </body>
    </html>

    <?php
}
?>