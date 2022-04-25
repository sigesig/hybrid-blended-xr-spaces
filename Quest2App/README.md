# AR-blended-workspaces
## Adding new Unity Projects
Follow this when adding new unity projects to Git.
First set up Git LFS to be able to version large projects.
> git lfs install

Now move/create the Unity project:
1. Move the entire Unity project folder into /AR-blended-workspaces
2. Copy .gitignore into your Unity project folder

Finally add the new project folder:
> git add /your-folder-name
>
> git commit -m "message"
>
> git push origin main