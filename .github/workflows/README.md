# GitHub Action Workflows

This folder contains the GitHub actions workflow files for this repository.

**Documentation:** <https://docs.github.com/en/actions/reference/workflow-syntax-for-github-actions>

## Security Considerations

* The `permissions` properties defines the permissions for the `GITHUB_TOKEN`. The default permission may be too permissive. So, it's a good idea to reduce the necessary permissions as much as possible.
  * Also, the default permission differ based on whether the workflow runs for a pull request from a fork or from the same repository.
  * Note that a regular build and test workflow usually only needs the `contents: read` permission.
  * For more details, see: <https://docs.github.com/en/actions/reference/authentication-in-a-workflow#permissions-for-the-github_token>
* For 3rd party actions (i.e. actions that don't come from `actions/`) it's recommended to use `@sha` rather than `@tag`. Reason: If the source repository gets hacked, the attacker can add malicious code (send secrets to they attacker's server) and change the tag to the commit with the malicious code. If you pin a certain `sha`, the action is not vulnerable to this type of attack.
  * Of course, with this you don't automatically get newer (good) versions of the action. Make sure to subscribe the action for new releases.
  * Technically, if the `GITHUB_TOKEN` permissions only include read permissions (and if you don't have any other secrets in your repository), it's not necessary to do this. But since nothing is every static, doing this even in this scenario is simply another safety net.
