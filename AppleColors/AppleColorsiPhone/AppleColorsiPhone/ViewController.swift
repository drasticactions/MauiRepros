//
//  ViewController.swift
//  AppleColorsiPhone
//
//  Created by DrasticActions on 2024/05/28.
//

import UIKit

class ViewController: UIViewController {
    
    override func viewDidLoad() {
        super.viewDidLoad()
        view.backgroundColor = UIColor.systemBackground
        let enabledButton = UIButton(type: .system)
        enabledButton.setTitle("⬛︎⬛︎⬛︎⬛︎⬛︎ Enabled Swift ⬛︎⬛︎⬛︎⬛︎⬛︎⬛︎", for: .normal)
        enabledButton.addTarget(self, action: #selector(enabledButtonTapped), for: .touchUpInside)
        enabledButton.translatesAutoresizingMaskIntoConstraints = false
        view.addSubview(enabledButton)
        
        // Set up the disabled button
        let disabledButton = UIButton(type: .system)
        disabledButton.setTitle("⬛︎⬛︎⬛︎⬛︎⬛︎ Disabled Swift ⬛︎⬛︎⬛︎⬛︎⬛︎⬛︎", for: .normal)
        disabledButton.isEnabled = false
        disabledButton.translatesAutoresizingMaskIntoConstraints = false
        view.addSubview(disabledButton)
        
        // Set up constraints
        NSLayoutConstraint.activate([
            enabledButton.centerXAnchor.constraint(equalTo: view.centerXAnchor),
            enabledButton.centerYAnchor.constraint(equalTo: view.centerYAnchor, constant: -20),
            
            disabledButton.centerXAnchor.constraint(equalTo: view.centerXAnchor),
            disabledButton.topAnchor.constraint(equalTo: enabledButton.bottomAnchor, constant: 20)
        ])
    }
    
    @objc func enabledButtonTapped() {
        print("Enabled button tapped")
    }
}

