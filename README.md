This system consisits of two seperate applications. One for the mobile phones(*MobileApp*) and one for the Oculus Quest 2(*Quest2App*).\
In order to build and run the mobile app, you should build the and run the MainScene which is located in MobileApp/Assets/Scenes.\
For the build settings, we used the same ones as proposed in the Augmented Reality course.
   - If you use Android: <details><summary>To get AR Foundation to build succesfully for Android you will need to change a few extra things (_click to expand_):</summary>
      1. First of all, with the **Build Settings** window open, click **Player Settings...** in the lower left.
      1. Expand the **Other Settings** dropdown.
         1. Under **Rendering** -> **Graphics APIs**, click **Vulkan**, then click the minus icon in the lower right of that box. This removes **Vulkan** as it is not supported on Android.
         1. Scroll down until you reach **Identification** -> **Minimun API Level**. Change this value to **Android 7.0 (API level 24)** _(If you have an older device you will need to download some additional things...)_
         1. Then change **Target API Level** to **Android 10.0 (API level 29)** _(If you have an older device you will need to download some additional things...)_
         1. Now find **Configuration** -> **Scripting Backend** and set that to **IL2CPP**.
         1. The find **Configuration** -> **Target Architectures**, untick **ARMv7** and tick **ARM64**. This is required as the current versions of ARCore no longer support 32bit.
         1. Now navigate to **XR Plug-in Management** in the left bar.
         1. Make sure the Android tab is selected and tick **ARCode** under **Plug-in Providers**.
         1. Then navigate to **XR Plug-in Management** -> **ARCore** in the left bar.
         1. Change the dropdown for **Depth** to **Optional**.
      1. You can now close the **Player Settings** window and go back to the **Build Settings** window.
      1. If you have yet to enable _USB Debugging_ on your Android phone, do the following:
         1. Open **Settings** -> Find **About Phone**.
         1. Click **Build Number** 5 or more times until it says **Developer Options** is enabled.
         1. Navigate to **Developer Options**, usually in **System**.
         1. Scroll down until you reach the **Debugging** section.
         1. Make sure **USB-Debugging** is enabled (**NOTE: You should disable this setting when not developing, as it makes your device more vulnerable...**)
      1. Plug your Android phone into your computer.
      1. Back in Unity in the **Build Settings** window, click the **Refresh** button on the right side.
         1. Then select your device in the dropdown to the left of that button.
         1. Click the **Build And Run** button which will prompt you for a save location of the application.
         1. Create a new folder called **Builds** and open it.
         1. Then type in a name in the bottom field and click **Save**.
         1. Unity will now build your application and it should appear on your phone.
      </details>

   - If you use iOS: <details><summary>To get AR Foundation to build succesfully for iOS you will need to change a few extra things (_click to expand_):</summary>
      1. Install Xcode (**App Store**: Search for Xcode and install)
      1. In Unity. Go to **Edit** -> **Project Settings** 
         1. -> **XR Plug-in Management**: tick **ARKit**
         1. -> **Player Settings** -> **iOS tab** (should be default if your target platform is iOS)
            1. Select a meaningful and kind of unique **Company name** and **Product Name** (it will create your bundle identifier; which will be com.CompanyName.ProductName which has to be unique to any other app in the world)
            1. **Other Settings** -> Tick **Requires ARKit Support**
            1. **Other Settings** -> **Architecture**: **ARM64** (might be default)
            1. **Other Settings** -> **Target minimun iOS Version**: **11.0**
      1. In Unity. Build and run your project (**File** -> **Build Settings** -> **Build and Run**)
         1. Create a `Build` folder in your project (e.g. `~/[Your PROJECT NAME]/AR22/Build`) to hold your project build files.
      1. Open project in Xcode
         1. **XCode** -> **Preferences** -> **Accounts**
            1. Add your AppleID by clicking **+**
            1. Click your AppleID -> **Manage Certificates**: add your laptop
         1. Connect your iPhone to your laptop
            1. Change **Any iOS Device** to your iPhone
            1. Click **Unity-iPhone project** -> **Signing & Capabilities**: click **Automatically manage signing**, and select **Team** to **[your name] (personal team)**
            1. Click play
      1. On your iPhone: **Settings** -> **General** -> **VPN & Administration**: allow your app
      </details>  

To run the Quest2 app, you should build the BlendedSpaces scene, which is located in QuestApp/Assets/Scenes. We used the build settings that is proposed here: https://developer.oculus.com/documentation/unity/unity-build/


Youtube Demo: https://www.youtube.com/watch?v=AJlQWN24jv4
