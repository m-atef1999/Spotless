import matplotlib.pyplot as plt
import pandas as pd
import seaborn as sns
import numpy as np

# Set style
sns.set_theme(style="whitegrid")
plt.rcParams['font.family'] = 'sans-serif'

# 1. Market Growth Chart (Egypt, Saudi, UAE)
def create_market_chart():
    data = {
        'Country': ['Saudi Arabia', 'Egypt', 'UAE'],
        'Market Size 2024 (Billion USD)': [5.4, 3.67, 2.0],
        'Projected 2033 (Billion USD)': [9.49, 6.73, 3.13]
    }
    df = pd.DataFrame(data)
    
    # Melt for grouped bar chart
    df_melted = df.melt(id_vars='Country', var_name='Year', value_name='Value')
    
    plt.figure(figsize=(10, 6))
    ax = sns.barplot(x='Country', y='Value', hue='Year', data=df_melted, palette='viridis')
    
    plt.title('Cleaning Services Market Growth in MENA (2024 vs 2033)', fontsize=16, pad=20)
    plt.ylabel('Market Size (Billion USD)', fontsize=12)
    plt.xlabel('Country', fontsize=12)
    
    # Add values on bars
    for container in ax.containers:
        ax.bar_label(container, fmt='%.1fB', padding=3)
        
    plt.tight_layout()
    plt.savefig('docs/market_growth_mena.png')
    plt.close()

# 2. Employment/Gig Economy Stats
def create_gig_stats_chart():
    # Data for Informal vs Formal Employment in Egypt (approx 64% informal)
    labels = ['Informal Employment', 'Formal Employment']
    sizes = [64, 36]
    colors = ['#ff9999', '#66b3ff']
    
    plt.figure(figsize=(8, 8))
    plt.pie(sizes, labels=labels, colors=colors, autopct='%1.1f%%', startangle=90, pctdistance=0.85, explode=(0.05, 0))
    
    # Draw circle
    centre_circle = plt.Circle((0,0), 0.70, fc='white')
    fig = plt.gcf()
    fig.gca().add_artist(centre_circle)
    
    plt.title('Egypt Workforce Composition: The Need for Formalization', fontsize=16)
    plt.tight_layout()
    plt.savefig('docs/employment_stats.png')
    plt.close()

# 3. Customer Pain Points Heatmap (Synthetic based on common issues)
def create_pain_points_heatmap():
    # Synthetic data represents 'Severity' of issues across different segments
    issues = ['Trust/Safety', 'Price Transparency', 'Availability', 'Quality Consistency']
    segments = ['Households', 'Offices', 'Busy Professionals']
    
    data = np.array([
        [9, 7, 8, 9],  # Households (High trust issue)
        [6, 9, 8, 7],  # Offices (High price/contract issue)
        [8, 6, 9, 8]   # Professionals (High availability issue)
    ])
    
    plt.figure(figsize=(10, 6))
    sns.heatmap(data, annot=True, xticklabels=issues, yticklabels=segments, cmap='Reds', vmin=0, vmax=10)
    plt.title('Customer Pain Points Severity Analysis', fontsize=16, pad=20)
    plt.tight_layout()
    plt.savefig('docs/pain_points_heatmap.png')
    plt.close()

if __name__ == "__main__":
    create_market_chart()
    create_gig_stats_chart()
    create_pain_points_heatmap()
