import itertools

import matplotlib.pyplot as plt
import numpy as np
import pingouin as pg
import scipy.stats as st




def posthoc_wilcoxon(A, B):
    stats = pg.wilcoxon(A, B)
    return stats


def posthoc_ttest(A, B):
    stats = pg.ttest(A, B, paired=True)
    return stats


def compare_all(conditions, posthoc_test=posthoc_wilcoxon):
    results = []
    # Running the test for all unique pairs without repetition
    for (name_A, A), (name_B, B) in itertools.combinations(conditions.items(), 2):
        result = posthoc_test(A, B)
        results.append({
            "Pair": f"{name_A} vs {name_B}",
            "Statistic": result["W-val" if posthoc_wilcoxon else "T"].to_numpy()[0],
            "P-value": result["p-val"].to_numpy()[0],
            "Effect Size": result["RBC" if posthoc_wilcoxon else "cohen-d"].to_numpy()[0]
        })
        
    p_vals = np.asarray([r["P-value"] for r in results])
    effect_sizes = np.asarray([r["Effect Size"] for r in results])
    
    return results, p_vals, effect_sizes


def make_plot(y_data, labels, xlabel, ylabel, save_figure_as=None, x_lines=None):

    species = labels
    
    # Create jittered version of "x" (which is only 0, 1, and 2)
    # More about this in the bonus track!
    jitter = 0.015  # 0.04
    x_data = [np.array([i] * len(d)) for i, d in enumerate(y_data)]
    x_jittered = [x + st.t(df=6, scale=jitter).rvs(len(x)) for x in x_data]
    
    title = f"Distribution of {ylabel} across {xlabel}s"
    data_range = np.sort(np.concatenate(y_data)).round(-1)
    HLINES = np.linspace(data_range[0], data_range[-1], 5).round(0) if x_lines is None else x_lines
    
    # Colors
    BG_WHITE = "#FFFFFF"  # "#fbf9f4"
    GREY_LIGHT = "#b4aea9"
    GREY50 = "#7F7F7F"
    BLUE_DARK = "#1B2838"
    BLUE = "#2a475e"
    BLACK = "#282724"
    GREY_DARK = "#747473"
    RED_DARK = "#850e00"
    
    # Colors taken from Dark2 palette in RColorBrewer R library
    COLOR_SCALE = ["#1B9E77", "#D95F02", "#7570B3", "#DE0077", "#DF9C09", "#535353"]
    
    # Horizontal positions for the violins. 
    # They are arbitrary numbers. They could have been [-1, 0, 1] for example.
    #POSITIONS = [0, 1]
    POSITIONS = list(range(len(species)))
    
    # Horizontal lines
    #HLINES = [40, 50, 60]
    
    fig, ax = plt.subplots(figsize= (14, 10))
    
    # Some layout stuff ----------------------------------------------
    # Background color
    fig.patch.set_facecolor(BG_WHITE)
    ax.set_facecolor(BG_WHITE)
    
    # Horizontal lines that are used as scale reference
    for h in HLINES:
        ax.axhline(h, color=GREY50, ls=(0, (5, 5)), alpha=0.8, zorder=0)
    
    # Add violins ----------------------------------------------------
    # bw_method="silverman" means the bandwidth of the kernel density
    # estimator is computed via Silverman's rule of thumb. 
    # More on this in the bonus track ;)
    
    # The output is stored in 'violins', used to customize their appearence
    violins = ax.violinplot(
        y_data, 
        positions=POSITIONS,
        widths=0.45,
        bw_method="silverman",
        showmeans=False, 
        showmedians=False,
        showextrema=False
    )
    
    # Customize violins (remove fill, customize line, etc.)
    for pc in violins["bodies"]:
        pc.set_facecolor("none")
        pc.set_edgecolor(GREY_LIGHT)#BLACK)
        pc.set_linewidth(1.4)
        pc.set_alpha(1)
    
    # Add boxplots ---------------------------------------------------
    # Note that properties about the median and the box are passed
    # as dictionaries.
    
    medianprops = dict(
        linewidth=4, 
        color=GREY_DARK,
        solid_capstyle="butt"
    )
    boxprops = dict(
        linewidth=2, 
        color=GREY_DARK
    )
    
    ax.boxplot(
        y_data,
        positions=POSITIONS, 
        showfliers = False, # Do not show the outliers beyond the caps.
        showcaps = False,   # Do not show the caps
        medianprops = medianprops,
        whiskerprops = boxprops,
        boxprops = boxprops
    )
    
    # Add jittered dots ----------------------------------------------
    for x, y, color in zip(x_jittered, y_data, COLOR_SCALE):
        ax.scatter(x, y, s = 100, color=color, alpha=0.4)

    ########################################################################
    
    # Add mean value labels ------------------------------------------
    means = [y.mean() for y in y_data]
    for i, mean in enumerate(means):
        # Add dot representing the mean
        ax.scatter(i, mean, s=250, color=RED_DARK, zorder=3)
       
        # Add line conecting mean value and its label
        ax.plot([i, i + 0.25], [mean, mean], ls="dashdot", color="black", zorder=3)
        
        # Add mean value label.
        ax.text(
            i + 0.25,
            mean,
            #r"$\hat{\mu}_{\rm{mean}} = $" + str(round(mean, 2)),
            str(round(mean, 2)),
            fontsize=18, #13,
            va="center",
            bbox = dict(
                facecolor="white",
                edgecolor="black",
                boxstyle="round",
                pad=0.15
            ),
            zorder=10 # to make sure the line is on top
        )
    
    # Add multiple comparisons p-value for mean difference -----------
    # Plot lines indicating what means are compared
    # 'tick_len' gives the length of the tick on the end of each line
      
    '''
    tick_len = 0.25
    ax.plot([0, 0, 1, 1], [62.5 - tick_len, 62.5, 62.5, 62.5 - tick_len], c="black")
    ax.plot([0, 0, 2, 2], [65 - tick_len, 65, 65, 65 - tick_len], c="black")
    ax.plot([1, 1, 2, 2], [67.5 - tick_len, 67.5, 67.5, 67.5 - tick_len], c="black")
    
    # Add labels for the p-values
    label1 = r"$p_{\rm{Holm-corrected}}$ = 8.42e-14"
    label2 = r"$p_{\rm{Holm-corrected}}$ = 4.3e-14"
    label3 = r"$p_{\rm{Holm-corrected}}$ = 0.031"
    
    pad = 0.2
    ax.text(0.5, 62.5 + pad, label1, fontsize=11, va="bottom", ha="center")
    ax.text(1, 65 + pad, label2, fontsize=11, va="bottom", ha="center")
    ax.text(1.5, 67.5 + pad, label3, fontsize=11, va="bottom", ha="center")  
      
    '''

    ########################################################################

    # Customize layout -----------------------------------------------
    # Hide spines
    ax.spines["right"].set_color("none")
    ax.spines["top"].set_color("none")
    
    # Customize spines color
    ax.spines["left"].set_color(GREY_LIGHT)
    ax.spines["left"].set_linewidth(2)
    ax.spines["bottom"].set_color(GREY_LIGHT)
    ax.spines["bottom"].set_linewidth(2)
    
    # Customize labels and ticks
    ax.tick_params(length=0)
    ax.set_yticks(HLINES)
    ax.set_yticklabels(HLINES, size=18)#15)
    ax.set_ylabel(ylabel, size=21, weight="bold") #18, weight="bold")
    
    # xlabels accounts for the sample size for each species
    xlabels = [' &\n'.join(specie.split(' & ')) + f"\n(n={y_data[i].size})" for i, specie in enumerate(species)]
    ax.set_xticks(POSITIONS)
    ax.set_xticklabels(xlabels, size=14, ha="center", ma="center") #18  #15, ha="center", ma="center")
    ax.set_xlabel(xlabel, size=21, weight="bold") #18, weight="bold")

    '''
    # Figure is used for the title and the axis is used for the subtitle
    # Title
    stats = [
        r"$\log_{\rm{e}}(\rm{BF}_{01})=-195.59$",
        r"$\widehat{R^2}_{\rm{Bayesian}}^{\rm{posterior}}=0.70$",
        r"$\rm{CI}_{95\%}^{\rm{HDI}}[0.67, 0.73]$",
        r"$r^{\rm{Cauchy}}_{\rm{JZS}} = 0.71$",
    ]
    
    fig.suptitle(
        title,
        x = 0.122,
        y = 0.975,
        ha="left",
        fontsize=22,  # 26,
        #fontname="Lobster Two",
        color=BLUE,
        weight="bold",    
    )

    # Subtitle   
    stats = [
        r"$F_{\rm{Welch}}$(2, 165.34)=409.93",
        r"p=8.27e-65",
        r"$\widehat{\omega_p^2}$=0.83",
        r"CI$_{95\%}$[0.79, 0.86]",
        r"n$_{\rm{obs}}$=333"
    ]
    
    ax.set_title(
        ", ".join(stats),
        loc="left",
        ha="left",
        fontsize=18,
        color=BLUE_DARK
    )
    '''
    '''
    # Notes on bottom-right
    fig.text(
        0.55,
        0.03,
        ", ".join(stats),
        fontsize=10
    )
    
    fig.text(
        0.55,
        0.005,
        r"Pairwise test: $\bf{Games-Howell}$ $\bf{test}$; Comparisons shown: $\bf{Only}$ $\bf{significant}$",
        fontsize=10
    )
    '''

    if save_figure_as is not None:
        fig.savefig(save_figure_as, format=save_figure_as.split('.')[-1])

    return fig
