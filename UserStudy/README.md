# Evaluation of the Study Results 
```bash
# Create conda env
conda create --name=menu-navigation python=3.11 -y
conda activate menu-navigation

conda install jupyter -y
pip install scipy numpy pandas statsmodels scipy matplotlib pingouin

# Run it
cd UserStudy
conda activate menu-navigation
jupyter notebook --ip 0.0.0.0 --no-browser --NotebookApp.custom_display_url="http://${HOSTNAME}:9898" --port 9898 --NotebookApp.token=password123
# Open browser and go to: http://127.0.0.1:9898/tree?token=password123
```